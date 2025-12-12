
using FallingDetectionService.Domain;
using FallingDetectionService.Infrastructure.Interfaces;
using FallingDetectionService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class IncidentService : IIncidentService
{
    private readonly SafetyDbContext _db;

    // Alert створюємо тільки для FALL (>= 7)
    private const float FALL_ALERT_THRESHOLD = 7.0f;

    public IncidentService(SafetyDbContext db)
    {
        _db = db;
    }

    private static DateTime EnsureUtc(DateTime dt)
    {
        if (dt.Kind == DateTimeKind.Unspecified)
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);

        return dt.ToUniversalTime();
    }

    private static IncidentType ResolveIncidentType(float confidence)
    {
        if (confidence < 5f)
            return IncidentType.INFO;

        if (confidence < 7f)
            return IncidentType.FALL_RISK;

        return IncidentType.FALL;
    }

    public async Task<Incident> IngestAsync(IncidentIngestPayload payload)
    {
        // --- Базова валідація ---
        if (payload.SiteId == Guid.Empty ||
            payload.ZoneId == Guid.Empty ||
            string.IsNullOrWhiteSpace(payload.SourceId) ||
            payload.Timestamp == default ||
            string.IsNullOrWhiteSpace(payload.EvidenceRef))
        {
            throw new ArgumentException("Invalid incident payload");
        }

        if (payload.Confidence < 0 || payload.Confidence > 10)
            throw new ArgumentException("Confidence must be in range 0..10");

        // --- FK-перевірки ---
        var siteExists = await _db.Sites.AnyAsync(s => s.Id == payload.SiteId);
        if (!siteExists)
            throw new KeyNotFoundException($"Site {payload.SiteId} not found.");

        var zoneExists = await _db.Zones.AnyAsync(z =>
            z.Id == payload.ZoneId && z.SiteId == payload.SiteId);

        if (!zoneExists)
            throw new KeyNotFoundException(
                $"Zone {payload.ZoneId} not found for site {payload.SiteId}.");

        // --- Нормалізація ---
        payload.Timestamp = EnsureUtc(payload.Timestamp);

        var incidentType = ResolveIncidentType(payload.Confidence);

        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            SiteId = payload.SiteId,
            ZoneId = payload.ZoneId,
            SourceId = payload.SourceId,
            Timestamp = payload.Timestamp,
            Type = incidentType,
            Confidence = payload.Confidence,
            EvidenceRef = payload.EvidenceRef,
            Status = IncidentStatus.NEW
        };

        await _db.Incidents.AddAsync(incident);

        // --- Alert створюємо ТІЛЬКИ для FALL ---
        if (incidentType == IncidentType.FALL)
        {
            var alert = new Alert
            {
                Id = Guid.NewGuid(),
                IncidentId = incident.Id,
                CreatedAt = DateTime.UtcNow,
                Level = "HIGH"
            };

            await _db.Alerts.AddAsync(alert);
        }

        await _db.SaveChangesAsync();
        return incident;
    }

    public async Task<Incident?> GetIncidentAsync(Guid id)
    {
        return await _db.Incidents
            .Include(i => i.Alerts)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IReadOnlyList<Incident>> ListIncidentsAsync(
        Guid siteId, DateTime start, DateTime end)
    {
        // важливо: очікуємо, що start/end вже UTC
        return await _db.Incidents
            .Where(i =>
                i.SiteId == siteId &&
                i.Timestamp >= start &&
                i.Timestamp <= end)
            .OrderBy(i => i.Timestamp)
            .ToListAsync();
    }
}
