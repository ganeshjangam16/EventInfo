using EventInfo.Client.Configuration;
using EventInfo.Client.Models;
using Ungerboeck.Api.Models.Subjects;
using Ungerboeck.Api.Sdk;

namespace EventInfo.Client.Services;

/// <summary>
/// Service for interacting with the Ungerboeck API
/// </summary>
public class UngerboeckClient : IUngerboeckClient
{
    private readonly ApiClient _apiClient;
    private readonly string _organizationCode;

    public UngerboeckClient(UngerboeckConfiguration configuration, string organizationCode)
    {
        var auth = new Ungerboeck.Api.Models.Authorization.Jwt
        {
            APIUserID = configuration.ApiUserId,
            Secret = configuration.Secret,
            Key = configuration.Key,
            UngerboeckURI = configuration.UngerboeckUri,
            AutoRefresh = new Ungerboeck.Api.Models.Authorization.AutoRefresh()
        };

        _apiClient = new ApiClient(auth);
        _organizationCode = organizationCode;
    }

    /// <summary>
    /// Gets high-level information for a specific event
    /// </summary>
    /// <param name="eventId">The event ID</param>
    /// <returns>Event information</returns>
    public async Task<EventInformation> GetEventAsync(int eventId)
    {
        var eventData = await Task.Run(() => _apiClient.Endpoints.Events.Get(_organizationCode, eventId));
        
        return new EventInformation
        {
            EventId = eventId,
            Description = eventData.Description,
            OrganizationCode = eventData.Organization,
            StartDate = eventData.StartDate,
            EndDate = eventData.EndDate,
            Status = eventData.Status,
            EventType = eventData.Type,
            Location = eventData.AnchorVenue
        };
    }

    /// <summary>
    /// Gets order headers (summary information) for a specific event
    /// </summary>
    /// <param name="eventId">The event ID</param>
    /// <returns>List of order headers for the event</returns>
    /// <remarks>
    /// This method attempts to retrieve order information. The exact endpoint and properties
    /// may vary depending on your Ungerboeck API version. You may need to adjust this method
    /// based on your specific Ungerboeck configuration.
    /// </remarks>
    public async Task<IEnumerable<OrderHeader>> GetOrderHeadersForEventAsync(int eventId)
    {
        try
        {
            // Attempt to search for orders by event ID using OData query
            // Note: This endpoint name may vary - check your Ungerboeck API documentation
            var searchValue = $"Event eq {eventId}";
            
            // Try using reflection to find the correct endpoint
            var endpointsType = _apiClient.Endpoints.GetType();
            var orderProperty = endpointsType.GetProperty("Orders") ?? endpointsType.GetProperty("OrderItems");
            
            if (orderProperty == null)
            {
                return Array.Empty<OrderHeader>();
            }
            
            dynamic orderEndpoint = orderProperty.GetValue(_apiClient.Endpoints);
            if (orderEndpoint == null)
            {
                return Array.Empty<OrderHeader>();
            }

            // Call Search method dynamically
            var searchMethod = orderEndpoint.GetType().GetMethod("Search");
            if (searchMethod == null)
            {
                return Array.Empty<OrderHeader>();
            }
            
            var orders = await Task.Run(() => searchMethod.Invoke(orderEndpoint, new object[] { _organizationCode, searchValue }));
            
            if (orders == null)
            {
                return Array.Empty<OrderHeader>();
            }
            
            // Process results - handle both direct collection and SearchResponse
            IEnumerable<dynamic> orderList;
            var ordersType = orders.GetType();
            var resultsProperty = ordersType.GetProperty("Results");
            
            if (resultsProperty != null)
            {
                orderList = (IEnumerable<dynamic>)resultsProperty.GetValue(orders);
            }
            else
            {
                orderList = (IEnumerable<dynamic>)orders;
            }

            // Group orders by OrderNumber to get unique orders (header level)
            var orderHeaders = orderList
                .GroupBy(o => (int)o.OrderNumber)
                .Select(g => g.First())
                .Select(order => new OrderHeader
                {
                    OrderNumber = GetPropertyValue<int>(order, "OrderNumber"),
                    OrganizationCode = GetPropertyValue<string>(order, "Organization"),
                    EventId = GetPropertyValue<int?>(order, "Event"),
                    AccountCode = GetPropertyValue<string>(order, "Account"),
                    Description = GetPropertyValue<string>(order, "Description"),
                    OrderDate = GetPropertyValue<DateTime?>(order, "OrderDate"),
                    Status = GetPropertyValue<string>(order, "Status") ?? GetPropertyValue<string>(order, "OrderStatus"),
                    OrderTotal = GetPropertyValue<decimal?>(order, "OrderTotal")
                });

            return orderHeaders;
        }
        catch (Exception ex)
        {
            // Log the exception for debugging, but return empty list for graceful degradation
            // This handles cases where Orders endpoint might not be available or configured
            System.Diagnostics.Debug.WriteLine($"Warning: Unable to retrieve orders for event {eventId}: {ex.Message}");
            return Array.Empty<OrderHeader>();
        }
    }

    private T? GetPropertyValue<T>(dynamic obj, string propertyName)
    {
        try
        {
            var type = obj.GetType();
            var property = type.GetProperty(propertyName);
            if (property != null)
            {
                var value = property.GetValue(obj);
                if (value != null)
                {
                    return (T)value;
                }
            }
        }
        catch
        {
            // Property doesn't exist or conversion failed - this is expected when checking
            // for properties that may not exist in all Ungerboeck API versions
        }
        return default(T);
    }

    /// <summary>
    /// Gets a list of all events for the organization
    /// </summary>
    /// <returns>List of events</returns>
    public async Task<IEnumerable<EventInformation>> GetEventsAsync()
    {
        // Use Search instead of GetList to retrieve events
        var events = await Task.Run(() => _apiClient.Endpoints.Events.Search(_organizationCode, string.Empty));

        return events.Results.Select(e => new EventInformation
        {
            EventId = e.EventID ?? 0,
            Description = e.Description,
            OrganizationCode = e.Organization,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Status = e.Status,
            EventType = e.Type,
            Location = e.AnchorVenue
        });
    }
}
