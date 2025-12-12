

@startuml
skinparam classAttributeIconSize 0
skinparam packageStyle rectangle
skinparam shadowing false

package "Domain" {

  class Site {
    +id: UUID
    +name: String
    +location: String
  }

  class Zone {
    +id: UUID
    +name: String
    +siteId: UUID
  }

  enum DeviceType {
    AI
    EDGE
    GATEWAY
  }

  class Device {
    +id: UUID
    +sourceId: String
    +type: DeviceType
    +siteId: UUID
    +metadata: String
  }

  class SensorReading {
    +id: UUID
    +timestamp: DateTime
    +rawData: String
  }

  enum IncidentType {
    FALL
    FALL_RISK
  }

  enum IncidentStatus {
    NEW
    ACKNOWLEDGED
  }

  class Incident {
    +id: UUID
    +siteId: UUID
    +zoneId: UUID
    +sourceId: String
    +timestamp: DateTime
    +type: IncidentType
    +confidence: Float
    +evidenceRef: String
    +status: IncidentStatus
  }

  class Alert {
    +id: UUID
    +incidentId: UUID
    +createdAt: DateTime
    +level: String
  }

}

package "Services" {

  class IncidentService {
    +ingest(payload): Incident
    +getIncident(id: UUID): Incident
    +listIncidents(siteId: UUID, start: DateTime, end: DateTime): List<Incident>
  }

  class DeviceService {
    +createDevice(sourceId:String, type:DeviceType, siteId:UUID, metadata:String): Device
    +getDevice(id:UUID): Device
  }

  class ReportService {
    +generateSafetyReport(siteId:UUID, start:DateTime, end:DateTime, format:String): File
  }

}


Site "1" -- "0..*" Zone
Site "1" -- "0..*" Device
Zone "1" -- "0..*" Incident
Device "1" -- "0..*" SensorReading
Incident "1" -- "0..1" Device : sourceId maps to >
Incident "1" -- "0..*" Alert

IncidentService --> Incident
IncidentService --> Alert
IncidentService --> SensorReading
DeviceService --> Device
ReportService --> Incident

note right of IncidentService
  Responsibilities:
  - validate minimal payload
  - persist Incident with status NEW
  - create Alert if confidence > code-threshold
  - return incidentId
end note

@enduml
