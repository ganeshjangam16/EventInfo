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

// Register Ungerboeck client as a singleton
builder.Services.AddSingleton<IUngerboeckClient>(sp => 
    new UngerboeckClient(ungerboeckConfig, defaultOrgCode));

// Add CORS if needed
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
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
