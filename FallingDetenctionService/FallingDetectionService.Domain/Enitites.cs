using System;
using FallingDetectionService.Domain;

namespace FallingDetectionService.Domain
{
    public class Site
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Location { get; set; } = default!;

        public ICollection<Zone> Zones { get; set; } = new List<Zone>();
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }

    public class Zone
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public Guid SiteId { get; set; }

        public Site Site { get; set; } = default!;
        public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    }

    public class Device
    {
        public Guid Id { get; set; }
        public string SourceId { get; set; } = default!;
        public DeviceType Type { get; set; }
        public Guid SiteId { get; set; }
        public string Metadata { get; set; } = default!;

        public Site Site { get; set; } = default!;
        public ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
    }

    public class SensorReading
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string RawData { get; set; } = default!;

        public Guid DeviceId { get; set; }
        public Device Device { get; set; } = default!;
    }

    public class Incident
    {
        public Guid Id { get; set; }
        public Guid SiteId { get; set; }
        public Guid ZoneId { get; set; }
        public string SourceId { get; set; } = default!;
        public DateTime Timestamp { get; set; }
        public IncidentType Type { get; set; }
        public float Confidence { get; set; }
        public string EvidenceRef { get; set; } = default!;
        public IncidentStatus Status { get; set; }

        public Zone Zone { get; set; } = default!;
        public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    }

    public class Alert
    {
        public Guid Id { get; set; }
        public Guid IncidentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Level { get; set; } = default!;

        public Incident Incident { get; set; } = default!;
    }

    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}
