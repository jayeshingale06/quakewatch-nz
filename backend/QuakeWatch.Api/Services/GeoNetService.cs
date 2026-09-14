using System.Text.Json;
using QuakeWatch.Api.Models;
using QuakeWatch.Api.Models.GeoNet;

namespace QuakeWatch.Api.Services;

public class GeoNetService
{
    private readonly HttpClient _http;
    private readonly ILogger<GeoNetService> _logger;

    // GeoNet's JSON casing varies, so match property names case-insensitively.
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
        var response = await _http.GetAsync($"quake?MMI={minMmi}");

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var geoNet = JsonSerializer.Deserialize<GeoNetResponse>(json, JsonOptions);

        if (geoNet?.Features is null)
        {
            _logger.LogWarning("GeoNet returned a response with no features");
            return new List<Quake>();
        }

        // Withdrawn events stay in the feed with quality "deleted". They are
        // usually false detections a reviewer rejected, so exclude them.
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
