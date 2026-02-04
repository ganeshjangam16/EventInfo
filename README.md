# EventInfo - Event Information API

A C# .NET 10 WebApi solution that provides high-level event information and order information (headers only) for events using the Ungerboeck API SDK.

## Solution Structure

The solution contains two projects:

### 1. EventInfo.Client (Class Library)
A class library that encapsulates all interactions with the Ungerboeck API SDK.

**Key Components:**
- **Configuration/UngerboeckConfiguration.cs**: Configuration class for Ungerboeck API credentials
- **Models/EventInformation.cs**: Model representing high-level event data
- **Models/OrderHeader.cs**: Model representing order header information
- **Services/IUngerboeckClient.cs**: Interface for the Ungerboeck client
- **Services/UngerboeckClient.cs**: Implementation that wraps the Ungerboeck SDK

### 2. EventInfo.Api (WebAPI)
ASP.NET Core Web API that exposes REST endpoints for accessing event and order information.

**Key Components:**
- **Controllers/EventsController.cs**: API controller with endpoints for events and orders
- **Program.cs**: Application startup and dependency injection configuration
- **appsettings.json**: Configuration including Ungerboeck API credentials

## Prerequisites

- .NET 10 SDK or later
- Access to an Ungerboeck instance
- Ungerboeck API User credentials (from Ungerboeck Main Menu -> Api Users)

## Configuration

1. Update `src/EventInfo.Api/appsettings.json` with your Ungerboeck credentials:

```json
{
  "Ungerboeck": {
    "UngerboeckUri": "https://yoursite.ungerboeck.com",
    "ApiUserId": "YOUR_API_USER_ID",
    "Secret": "YOUR_SECRET_GUID",
    "Key": "YOUR_KEY_GUID",
    "DefaultOrganizationCode": "10"
  }
}
```

**Important:** For production, use User Secrets, Environment Variables, or Azure Key Vault instead of storing credentials in appsettings.json.

### Finding Your Credentials

1. Log into Ungerboeck
2. Navigate to Main Menu -> Api Users
3. Select or create an API User
4. Copy the following values:
   - **API User ID**: The ID displayed in the API User details
   - **Secret**: A GUID value from the API User details window
   - **Key**: One of the Key values (GUID) from the Keys section
   - **Organization Code**: Your organization code (e.g., "10")

## Building the Solution

```bash
# Navigate to the solution directory
cd /path/to/EventInfo

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Build in Release mode
dotnet build -c Release
```

## Running the API

```bash
# Navigate to the API project
cd src/EventInfo.Api

# Run the application
dotnet run
```

The API will start and listen on:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

## API Endpoints

### Get All Events
```
GET /api/events
```
Returns a list of all events for the configured organization.

### Get Specific Event
```
GET /api/events/{eventId}
```
Returns high-level information for a specific event.

**Parameters:**
- `eventId` (int): The event ID

**Response Example:**
```json
{
  "eventId": 12345,
  "description": "Annual Conference 2024",
  "organizationCode": "10",
  "startDate": "2024-03-15T09:00:00",
  "endDate": "2024-03-17T17:00:00",
  "status": "30",
  "eventType": "CONF",
  "location": "HALL-A"
}
```

### Get Event Orders
```
GET /api/events/{eventId}/orders
```
Returns order headers (summary information) for a specific event.

**Parameters:**
- `eventId` (int): The event ID

**Response Example:**
```json
[
  {
    "orderNumber": 1001,
    "organizationCode": "10",
    "eventId": 12345,
    "accountCode": "ACCT001",
    "description": "Catering Order",
    "orderDate": "2024-02-20T10:30:00",
    "status": "Active",
    "orderTotal": 5000.00
  }
]
```

## Testing the API

### Using curl
```bash
# Get all events
curl https://localhost:5001/api/events

# Get specific event
curl https://localhost:5001/api/events/12345

# Get orders for an event
curl https://localhost:5001/api/events/12345/orders
```

### Using OpenAPI/Swagger (Development Mode)
When running in Development mode, navigate to:
```
https://localhost:5001/openapi/v1.json
```

## Dependencies

### EventInfo.Client
- **Ungerboeck.Api.Sdk** (1.254.1.4): Official Ungerboeck API SDK
  - Provides access to Ungerboeck API endpoints
  - Includes JWT authentication
  - Contains pre-made models and constants

### EventInfo.Api
- **EventInfo.Client**: Reference to the client library
- **Microsoft.AspNetCore.OpenApi**: For OpenAPI documentation

## Development

### Project Structure
```
EventInfo/
├── EventInfo.sln
├── README.md
├── .gitignore
└── src/
    ├── EventInfo.Client/
    │   ├── Configuration/
    │   │   └── UngerboeckConfiguration.cs
    │   ├── Models/
    │   │   ├── EventInformation.cs
    │   │   └── OrderHeader.cs
    │   └── Services/
    │       ├── IUngerboeckClient.cs
    │       └── UngerboeckClient.cs
    └── EventInfo.Api/
        ├── Controllers/
        │   └── EventsController.cs
        ├── Program.cs
        └── appsettings.json
```

### Adding New Features

To add support for additional Ungerboeck entities:

1. Add new model classes in `EventInfo.Client/Models/`
2. Add methods to `IUngerboeckClient` interface
3. Implement methods in `UngerboeckClient`
4. Create new controllers in `EventInfo.Api/Controllers/`

## Notes

- The Orders endpoint implementation uses reflection to handle different Ungerboeck API versions, as the exact endpoint name may vary
- All API calls are wrapped in try-catch blocks with appropriate error logging
- The solution uses JWT authentication automatically through the Ungerboeck SDK
- CORS is enabled by default for development purposes

## Troubleshooting

### Configuration Errors
If you see "Ungerboeck configuration is missing", verify:
- appsettings.json contains the "Ungerboeck" section
- All required fields are populated
- JSON syntax is valid

### Authentication Errors
If you receive 401 Unauthorized errors:
- Verify API credentials are correct
- Check that the API User is active in Ungerboeck
- Ensure the Key value is from the Keys section

### Connection Errors
If you cannot connect to Ungerboeck:
- Verify the UngerboeckUri is correct
- Check network connectivity
- Ensure firewall allows outbound HTTPS connections

## Resources

- [Ungerboeck API SDK NuGet Package](https://www.nuget.org/packages/Ungerboeck.Api.Sdk/)
- [Ungerboeck API Examples (GitHub)](https://github.com/UngerboeckAPI/254)
- [Ungerboeck Support Center](https://supportcenter.ungerboeck.com/hc/en-us/sections/115001365327-API-Basics)

## License

This project is provided as-is for integration with Ungerboeck systems.
