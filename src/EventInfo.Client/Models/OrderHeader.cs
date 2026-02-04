namespace EventInfo.Client.Models;

/// <summary>
/// Represents order header information for an event
/// </summary>
public class OrderHeader
{
    public int OrderNumber { get; set; }
    public string? OrganizationCode { get; set; }
    public int? EventId { get; set; }
    public string? AccountCode { get; set; }
    public string? Description { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? Status { get; set; }
    public decimal? OrderTotal { get; set; }
}
