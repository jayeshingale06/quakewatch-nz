using Microsoft.EntityFrameworkCore;
using QuakeWatch.Api.Data;
using QuakeWatch.Api.Models;

namespace QuakeWatch.Api.Services;

// Decides where earthquakes come from:
// fresh copy in the database  -> use it
// old or empty database       -> refresh from GeoNet first

public class QuakeStore
{
    private static readonly TimeSpan CacheLifetime = TimeSpan.FromMinutes(10);

    private readonly QuakeDbContext _db;
    private readonly GeoNetService _geoNet;
    private readonly ILogger<QuakeStore> _logger;

    public QuakeStore(
        QuakeDbContext db,
        GeoNetService geoNet,
        ILogger<QuakeStore> logger)
    {
        _db = db;
        _geoNet = geoNet;
        _logger = logger;
    }

    public async Task<List<Quake>> GetQuakesAsync(int minMmi)
    {
        if (await IsStaleAsync())
        {
            _logger.LogInformation("Cache is stale. Refreshing from GeoNet.");
            await RefreshFromGeoNetAsync();
        }
        else
        {
            _logger.LogInformation("Cache is fresh. Serving from the database.");
        }

        var rows = await _db.Quakes
            .Where(row => row.Mmi >= minMmi)
            .OrderByDescending(row => row.TimeUtc)
            .ToListAsync();

        return rows.Select(ToQuake).ToList();
    }

    private async Task<bool> IsStaleAsync()
    {
        // Nothing stored yet, so there is nothing to serve.
        if (!await _db.Quakes.AnyAsync())
        {
            return true;
        }

        var lastFetched = await _db.Quakes.MaxAsync(row => row.FetchedAtUtc);

        return DateTime.UtcNow - lastFetched > CacheLifetime;
    }

    private async Task RefreshFromGeoNetAsync()
    {
        var fresh = await _geoNet.GetQuakesAsync(minMmi: 0);
        var now = DateTime.UtcNow;

        foreach (var quake in fresh)
        {
            // FindAsync looks up by primary key.
            var existing = await _db.Quakes.FindAsync(quake.PublicID);

            if (existing is null)
            {
                // New earthquake: add a row.
                _db.Quakes.Add(new QuakeEntity
                {
                    PublicID = quake.PublicID,
                    Magnitude = quake.Magnitude,
                    Depth = quake.Depth,
                    Locality = quake.Locality,
                    Mmi = quake.Mmi,
                    Quality = quake.Quality,
                    TimeUtc = quake.Time.UtcDateTime,
                    FetchedAtUtc = now,
                });
            }
            else
            {
                // Already known: update it. GeoNet revises quakes as
                // humans review them, so quality and magnitude change.
                existing.Magnitude = quake.Magnitude;
                existing.Depth = quake.Depth;
                existing.Locality = quake.Locality;
                existing.Mmi = quake.Mmi;
                existing.Quality = quake.Quality;
                existing.TimeUtc = quake.Time.UtcDateTime;
                existing.FetchedAtUtc = now;
            }
        }

        // Nothing above touched the database. THIS line does it all,
        // in one transaction.
        var changes = await _db.SaveChangesAsync();

        _logger.LogInformation("Wrote {Changes} rows to the database.", changes);
    }

    private static Quake ToQuake(QuakeEntity row) => new Quake(
        PublicID: row.PublicID,
        Magnitude: row.Magnitude,
        Depth: row.Depth,
        Locality: row.Locality,
        Mmi: row.Mmi,
        Quality: row.Quality,
        Time: new DateTimeOffset(DateTime.SpecifyKind(row.TimeUtc, DateTimeKind.Utc))
    );
}
