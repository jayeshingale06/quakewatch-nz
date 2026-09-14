namespace QuakeWatch.Api.Data;

// EF Core creates these itself and fills one property at a time, so this is a
// class with settable properties rather than a record.
// Times are stored as UTC DateTime because SQLite has no real date type.
public class QuakeEntity
{
    public string PublicID { get; set; } = "";
    public double Magnitude { get; set; }
    public double Depth { get; set; }
    public string Locality { get; set; } = "";
    public int Mmi { get; set; }
    public string Quality { get; set; } = "";

    public DateTime TimeUtc { get; set; }

    // When this row was last copied from GeoNet. Drives cache staleness.
    public DateTime FetchedAtUtc { get; set; }
}
