using Microsoft.EntityFrameworkCore;

namespace QuakeWatch.Api.Data;

// A DbContext is your conversation with the database.
// One property per table.

public class QuakeDbContext : DbContext
{
    public QuakeDbContext(DbContextOptions<QuakeDbContext> options)
        : base(options)
    {
    }

    // This one property IS the Quakes table.
    public DbSet<QuakeEntity> Quakes => Set<QuakeEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var quake = modelBuilder.Entity<QuakeEntity>();

        // PublicID is GeoNet's own unique id, so we use it as the
        // primary key instead of inventing a number of our own.
        quake.HasKey(row => row.PublicID);

        // Indexes make the two things we search by fast.
        quake.HasIndex(row => row.Mmi);
        quake.HasIndex(row => row.TimeUtc);
    }
}
