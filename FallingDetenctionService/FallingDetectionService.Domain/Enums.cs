namespace FallingDetectionService.Domain
{
    public enum DeviceType
    {
        AI,
        EDGE,
        GATEWAY
    }

    public enum IncidentType
    {
        INFO,        
        FALL_RISK,   
        FALL         
    }

    public enum IncidentStatus
    {
        NEW,
        ACKNOWLEDGED
    }
}