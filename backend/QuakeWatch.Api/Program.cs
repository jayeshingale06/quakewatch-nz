using Microsoft.EntityFrameworkCore;
using QuakeWatch.Api.Data;
using QuakeWatch.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Hosting platforms such as Render set PORT and expect the app to use it.
// Compose and Container Apps do not, so fall back to the image default.
var port = Environment.GetEnvironmentVariable("PORT");

if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://+:{port}");
}

// A connection string means PostgreSQL. No connection string means a local SQLite file.
var connectionString = builder.Configuration.GetConnectionString("Default");

// The container runs as a non-root user and cannot write to /app, so the
// Dockerfile points this at a directory it owns.
var sqlitePath = builder.Configuration["Sqlite:Path"] ?? "quakewatch.db";

builder.Services.AddDbContext<QuakeDbContext>(options =>
{
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        options.UseSqlite($"Data Source={sqlitePath}");
    }
    else
    {
        options.UseNpgsql(connectionString);
    }
});

builder.Services.AddHttpClient<GeoNetService>(client =>
{
    client.BaseAddress = new Uri("https://api.geonet.org.nz/");
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.TryAddWithoutValidation(
        "Accept", "application/vnd.geo+json;version=2");
});

builder.Services.AddScoped<QuakeStore>();

// Origins come from configuration so Docker and Azure can be changed without
// touching code.
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuakeDbContext>();
    db.Database.EnsureCreated();
}

app.Logger.LogInformation(
    "Database provider: {Provider}",
    string.IsNullOrWhiteSpace(connectionString)
        ? $"SQLite at {sqlitePath}"
        : "PostgreSQL");

app.Logger.LogInformation(
    "CORS allows {Count} origin(s)", allowedOrigins.Length);

app.UseCors("frontend");

app.MapGet("/", () => "QuakeWatch API is running");

// Liveness probe for the container host. Must be fast and must not call GeoNet.
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/api/quakes", async (
    QuakeStore store,
    ILogger<Program> logger,
    int? minMmi) =>
{
    try
    {
        var quakes = await store.GetQuakesAsync(minMmi ?? 3);
        return Results.Ok(quakes);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Could not get quakes");
        return Results.Problem(
            detail: "Could not load earthquakes right now. Try again shortly.",
            statusCode: 503);
    }
});

app.Run();
