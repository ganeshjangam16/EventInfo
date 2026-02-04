namespace EventInfo.Client.Models;

/// <summary>
/// Represents high-level event information
/// </summary>
public class EventInformation
{
    public int EventId { get; set; }
    public string? Description { get; set; }
    public string? OrganizationCode { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public string? EventType { get; set; }
    public string? Location { get; set; }
}
