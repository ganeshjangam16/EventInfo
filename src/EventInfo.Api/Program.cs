using EventInfo.Client.Configuration;
using EventInfo.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure Ungerboeck settings
var ungerboeckConfig = builder.Configuration.GetSection("Ungerboeck").Get<UngerboeckConfiguration>() 
    ?? throw new InvalidOperationException("Ungerboeck configuration is missing");

var defaultOrgCode = builder.Configuration["Ungerboeck:DefaultOrganizationCode"] 
    ?? throw new InvalidOperationException("DefaultOrganizationCode is required");

// Register Ungerboeck client as scoped (one instance per request)
// This ensures proper resource management and connection handling
builder.Services.AddScoped<IUngerboeckClient>(sp => 
    new UngerboeckClient(ungerboeckConfig, defaultOrgCode));

// Add CORS with environment-based configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Allow all origins in development for easier testing
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // In production, restrict to specific origins
            // Configure allowed origins via appsettings.json or environment variables
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                ?? Array.Empty<string>();
            
            if (allowedOrigins.Length > 0)
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors();

app.MapControllers();

app.Run();
