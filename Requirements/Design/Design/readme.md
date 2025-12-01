

@startuml
skinparam classAttributeIconSize 0
skinparam packageStyle rectangle
skinparam shadowing false

package "Domain" {

  class User {
    +id: UUID
    +name: String
    +email: String
    +role: Role
  }

  enum Role {
    STAKEHOLDER
    PRODUCT_OWNER
    ARCHITECT
    DEVELOPER
    ADMIN
  }

  class ConstructionSite {
    +id: UUID
    +name: String
    +location: String
  }

  class Zone {
    +id: UUID
    +name: String
    +riskLevel: RiskLevel
  }

  enum RiskLevel {
    LOW
    MEDIUM
    HIGH
  }

  class IoTDevice {
    +id: UUID
    +deviceId: String
    +type: DeviceType
    +status: DeviceStatus
    +zone: Zone
  }

  enum DeviceType {
    SENSOR
    CAMERA
    TRACKER
  }

  enum DeviceStatus {
    ACTIVE
    INACTIVE
    ERROR
  }

  class Incident {
    +id: UUID
    +timestamp: DateTime
    +status: IncidentStatus
    +type: IncidentType
    +location: String
    +comment: String
    +confirm(user: User)
    +markAsFalseAlarm(user: User, reason: String)
  }

  enum IncidentStatus {
    NEW
    CONFIRMED
    FALSE_ALARM
    ESCALATED
  }

  enum IncidentType {
    FALL
    FALL_RISK
  }

  class SensorReading {
    +id: UUID
    +timestamp: DateTime
    +rawData: String
  }

  class Alert {
    +id: UUID
    +createdAt: DateTime
    +deliveredAt: DateTime
    +channel: NotificationChannel
  }

  enum NotificationChannel {
    WEB
    EMAIL
    SMS
    PUSH
  }

  class Report {
    +id: UUID
    +periodStart: DateTime
    +periodEnd: DateTime
    +generatedAt: DateTime
    +format: ReportFormat
  }

  enum ReportFormat {
    PDF
    XLSX
  }

}

package "Services" {

  class AuthService {
    +login(email: String, password: String): User
    +logout(user: User): void
  }

  class IncidentService {
    +createIncident(reading: SensorReading, site: ConstructionSite, zone: Zone): Incident
    +confirmIncident(incidentId: UUID, user: User): void
    +markFalseAlarm(incidentId: UUID, user: User, reason: String): void
    +getIncidents(siteId: UUID): List<Incident>
  }

  class IoTDeviceService {
    +registerDevice(deviceId: String, type: DeviceType, zone: Zone): IoTDevice
    +testConnection(deviceId: String): boolean
    +updateStatus(deviceId: String, status: DeviceStatus): void
  }

  class NotificationService {
    +sendAlert(incident: Incident, recipients: List<User>): void
  }

  class AIAnalyzer {
    +analyze(reading: SensorReading): IncidentType
    +isFallDetected(reading: SensorReading): boolean
  }

  class ReportService {
    +generateSafetyReport(site: ConstructionSite, from: DateTime, to: DateTime): Report
  }
}

 
User "1" -- "0..*" Incident : actsOn >
ConstructionSite "1" -- "0..*" Zone
Zone "1" -- "0..*" IoTDevice
IoTDevice "1" -- "0..*" SensorReading
Incident "1" -- "0..*" Alert
Incident "*" --> "1" ConstructionSite
Incident "*" --> "1" Zone

 
IncidentService --> Incident
IncidentService --> SensorReading
IncidentService --> ConstructionSite
IncidentService --> Zone
IncidentService --> NotificationService

IoTDeviceService --> IoTDevice
NotificationService --> Alert
NotificationService --> User
AIAnalyzer --> SensorReading
AIAnalyzer --> IncidentType
ReportService --> Report
ReportService --> Incident
AuthService --> User

@enduml
