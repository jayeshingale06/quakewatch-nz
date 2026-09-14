using Microsoft.EntityFrameworkCore;

namespace QuakeWatch.Api.Data;

public class QuakeDbContext : DbContext
{
    public QuakeDbContext(DbContextOptions<QuakeDbContext> options)
        : base(options)
    {
    }

    public DbSet<QuakeEntity> Quakes => Set<QuakeEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var quake = modelBuilder.Entity<QuakeEntity>();

        // PublicID is GeoNet's own unique id, so it doubles as the primary key.
        quake.HasKey(row => row.PublicID);

        // Both columns are used for filtering and ordering.
        quake.HasIndex(row => row.Mmi);
        quake.HasIndex(row => row.TimeUtc);
    }
}
