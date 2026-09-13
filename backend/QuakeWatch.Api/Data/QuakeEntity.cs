namespace QuakeWatch.Api.Data;

// The shape stored in the DATABASE, one object per row.
//
// This is a plain class with { get; set; }, not a record, because
// Entity Framework creates these itself and fills them in one
// property at a time. It needs permission to set each one.
//
// Times are stored as plain UTC DateTime, because SQLite has no
// real date type and UTC avoids every timezone problem.

public class QuakeEntity
{
    public string PublicID { get; set; } = "";
    public double Magnitude { get; set; }
    public double Depth { get; set; }
    public string Locality { get; set; } = "";
    public int Mmi { get; set; }
    public string Quality { get; set; } = "";

    // When the earthquake happened.
    public DateTime TimeUtc { get; set; }

    // When WE last copied this row from GeoNet. Used to decide
    // whether our copy is old enough to be worth refreshing.
    public DateTime FetchedAtUtc { get; set; }
}
