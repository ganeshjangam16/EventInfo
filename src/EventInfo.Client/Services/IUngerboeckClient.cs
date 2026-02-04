using EventInfo.Client.Models;

namespace EventInfo.Client.Services;

/// <summary>
/// Interface for the Ungerboeck API client
/// </summary>
public interface IUngerboeckClient
{
    /// <summary>
    /// Gets high-level information for a specific event
    /// </summary>
    /// <param name="eventId">The event ID</param>
    /// <returns>Event information</returns>
    Task<EventInformation> GetEventAsync(int eventId);

    /// <summary>
    /// Gets order headers (summary information) for a specific event
    /// </summary>
    /// <param name="eventId">The event ID</param>
    /// <returns>List of order headers for the event</returns>
    Task<IEnumerable<OrderHeader>> GetOrderHeadersForEventAsync(int eventId);

    /// <summary>
    /// Gets a list of all events for the organization
    /// </summary>
    /// <returns>List of events</returns>
    Task<IEnumerable<EventInformation>> GetEventsAsync();
}
