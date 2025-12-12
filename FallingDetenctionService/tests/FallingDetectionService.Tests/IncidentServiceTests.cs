using FallingDetectionService.Domain;
using FallingDetectionService.Infrastructure.Interfaces; // IncidentIngestPayload
using FallingDetectionService.Infrastructure.Services; // IncidentService
using FallingDetectionService.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FallingDetectionService.Tests;

public class IncidentServiceTests
{
    private static async Task<(Guid siteId, Guid zoneId)> SeedSiteAndZoneAsync(
        FallingDetectionService.Infrastructure.Persistence.SafetyDbContext db)
    {
        var siteId = Guid.NewGuid();
        var zoneId = Guid.NewGuid();

        db.Sites.Add(new Site
        {
            Id = siteId,
            Name = "Test Site",
            Location = "Test Location"
        });

        db.Zones.Add(new Zone
        {
            Id = zoneId,
            Name = "Zone A",
            SiteId = siteId
        });

        await db.SaveChangesAsync();
        return (siteId, zoneId);
    }

    [Theory]
    [InlineData(4.99f, IncidentType.INFO, 0)]
    [InlineData(5.00f, IncidentType.FALL_RISK, 0)]
    [InlineData(6.99f, IncidentType.FALL_RISK, 0)]
    [InlineData(7.00f, IncidentType.FALL, 1)]
    [InlineData(10.00f, IncidentType.FALL, 1)]
    public async Task IngestAsync_ShouldAssignType_AndCreateAlert_ByConfidence(
        float confidence, IncidentType expectedType, int expectedAlerts)
    {
        // Arrange
        var (db, conn) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = db;
        await using var __ = conn;

        var (siteId, zoneId) = await SeedSiteAndZoneAsync(db);

        var sut = new IncidentService(db);

        var payload = new IncidentIngestPayload
        {
            SiteId = siteId,
            ZoneId = zoneId,
            SourceId = "camera-001",
            Timestamp = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Unspecified), // спеціально Unspecified
            Confidence = confidence,
            EvidenceRef = "demo://evidence",
            Type = IncidentType.FALL // буде проігноровано сервісом (за вашим правилом)
        };

        // Act
        var created = await sut.IngestAsync(payload);

        // Assert
        created.Type.Should().Be(expectedType);
        created.Status.Should().Be(IncidentStatus.NEW);

        // Timestamp має бути UTC після EnsureUtc
        created.Timestamp.Kind.Should().Be(DateTimeKind.Utc);

        var alerts = await db.Alerts.Where(a => a.IncidentId == created.Id).ToListAsync();
        alerts.Count.Should().Be(expectedAlerts);
    }

    [Fact]
    public async Task IngestAsync_ConfidenceOutOfRange_ShouldThrowArgumentException()
    {
        // Arrange
        var (db, conn) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = db;
        await using var __ = conn;

        var (siteId, zoneId) = await SeedSiteAndZoneAsync(db);
        var sut = new IncidentService(db);

        var payload = new IncidentIngestPayload
        {
            SiteId = siteId,
            ZoneId = zoneId,
            SourceId = "camera-001",
            Timestamp = DateTime.UtcNow,
            Confidence = 10.01f, // поза 0..10
            EvidenceRef = "demo://evidence",
            Type = IncidentType.INFO
        };

        // Act
        Func<Task> act = () => sut.IngestAsync(payload);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Confidence*");
    }

    [Fact]
    public async Task IngestAsync_SiteDoesNotExist_ShouldThrowKeyNotFound()
    {
        // Arrange
        var (db, conn) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = db;
        await using var __ = conn;

        var sut = new IncidentService(db);

        var payload = new IncidentIngestPayload
        {
            SiteId = Guid.NewGuid(), // неіснуючий
            ZoneId = Guid.NewGuid(),
            SourceId = "camera-001",
            Timestamp = DateTime.UtcNow,
            Confidence = 7.5f,
            EvidenceRef = "demo://evidence",
            Type = IncidentType.FALL
        };

        // Act
        Func<Task> act = () => sut.IngestAsync(payload);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Site*not found*");
    }

    [Fact]
    public async Task IngestAsync_ZoneDoesNotExistForSite_ShouldThrowKeyNotFound()
    {
        // Arrange
        var (db, conn) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = db;
        await using var __ = conn;

        // створюємо Site, але Zone іншу/відсутню
        var siteId = Guid.NewGuid();
        db.Sites.Add(new Site { Id = siteId, Name = "S", Location = "L" });
        await db.SaveChangesAsync();

        var sut = new IncidentService(db);

        var payload = new IncidentIngestPayload
        {
            SiteId = siteId,
            ZoneId = Guid.NewGuid(), // неіснуюча zone
            SourceId = "camera-001",
            Timestamp = DateTime.UtcNow,
            Confidence = 6.0f,
            EvidenceRef = "demo://evidence",
            Type = IncidentType.FALL_RISK
        };

        // Act
        Func<Task> act = () => sut.IngestAsync(payload);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Zone*not found*");
    }
}
