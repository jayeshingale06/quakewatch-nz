using System.Text.Json;
using QuakeWatch.Api.Models;
using QuakeWatch.Api.Models.GeoNet;

namespace QuakeWatch.Api.Services;

// This class has ONE job: get earthquakes from GeoNet
// and hand them back in OUR shape.

public class GeoNetService
{
    private readonly HttpClient _http;
    private readonly ILogger<GeoNetService> _logger;

    // Match JSON names ignoring capital letters, so "mmi", "Mmi"
    // and "MMI" all land in our Mmi property.
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public GeoNetService(HttpClient http, ILogger<GeoNetService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<List<Quake>> GetQuakesAsync(int minMmi)
    {
        // 1. Ask GeoNet. This takes time, so we await it.
        var response = await _http.GetAsync($"quake?MMI={minMmi}");

        // 2. Stop here if GeoNet said no.
        response.EnsureSuccessStatusCode();

        // 3. Read the answer as text.
        var json = await response.Content.ReadAsStringAsync();

        // 4. Turn that text into C# objects.
        var geoNet = JsonSerializer.Deserialize<GeoNetResponse>(json, JsonOptions);

        if (geoNet?.Features is null)
        {
            _logger.LogWarning("GeoNet returned a response with no features");
            return new List<Quake>();
        }

        // 5. Convert THEIR shape into OUR shape, newest first.
        //
        // GeoNet marks withdrawn events with quality "deleted". These are
        // usually false detections a human later rejected. They are not
        // real earthquakes and must not be displayed.
        return geoNet.Features
            .Where(feature => feature.Properties is not null)
            .Where(feature => !IsWithdrawn(feature.Properties!))
            .Select(feature => ToQuake(feature.Properties!))
            .OrderByDescending(quake => quake.Time)
            .ToList();
    }

    private static bool IsWithdrawn(GeoNetProperties p) =>
        string.Equals(p.Quality, "deleted", StringComparison.OrdinalIgnoreCase);

    private static Quake ToQuake(GeoNetProperties p) => new Quake(
        PublicID: p.PublicID ?? "unknown",
        Magnitude: Math.Round(p.Magnitude, 1),
        Depth: Math.Round(p.Depth, 1),
        Locality: p.Locality ?? "Unknown location",
        Mmi: p.Mmi,
        Quality: p.Quality ?? "unknown",
        Time: p.Time
    );
}
