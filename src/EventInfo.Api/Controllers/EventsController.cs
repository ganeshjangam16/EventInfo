using EventInfo.Client.Models;
using EventInfo.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventInfo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IUngerboeckClient _ungerboeckClient;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IUngerboeckClient ungerboeckClient, ILogger<EventsController> logger)
    {
        _ungerboeckClient = ungerboeckClient;
        _logger = logger;
    }

    /// <summary>
    /// Gets high-level information for a specific event
    /// </summary>
    /// <param name="eventId">The event ID</param>
    /// <returns>Event information</returns>
    [HttpGet("{eventId}")]
    [ProducesResponseType(typeof(EventInformation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EventInformation>> GetEvent(int eventId)
    {
        try
        {
            _logger.LogInformation("Retrieving event {EventId}", eventId);
            var eventInfo = await _ungerboeckClient.GetEventAsync(eventId);
            return Ok(eventInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving event {EventId}", eventId);
            return StatusCode(500, new { error = "An error occurred while retrieving the event" });
        }
    }

    /// <summary>
    /// Gets order headers (summary information) for a specific event
    /// </summary>
    /// <param name="eventId">The event ID</param>
    /// <returns>List of order headers for the event</returns>
    [HttpGet("{eventId}/orders")]
    [ProducesResponseType(typeof(IEnumerable<OrderHeader>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<OrderHeader>>> GetEventOrders(int eventId)
    {
        try
        {
            _logger.LogInformation("Retrieving orders for event {EventId}", eventId);
            var orders = await _ungerboeckClient.GetOrderHeadersForEventAsync(eventId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders for event {EventId}", eventId);
            return StatusCode(500, new { error = "An error occurred while retrieving event orders" });
        }
    }

    /// <summary>
    /// Gets a list of all events for the organization
    /// </summary>
    /// <returns>List of events</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventInformation>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<EventInformation>>> GetAllEvents()
    {
        try
        {
            _logger.LogInformation("Retrieving all events");
            var events = await _ungerboeckClient.GetEventsAsync();
            return Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events");
            return StatusCode(500, new { error = "An error occurred while retrieving events" });
        }
    }
}
