namespace QuakeWatch.Api.Models;

// A Quake is a SHAPE.
// Every earthquake in this app must have exactly these seven pieces
// of information, each of the exact type written here. The compiler
// checks this everywhere the shape is used.

public record Quake(
    string PublicID,
    double Magnitude,
    double Depth,
    string Locality,
    int Mmi,
    string Quality,
    DateTimeOffset Time
)
{
    // Severity is WORKED OUT from Mmi, never stored.
    // Bands based on GeoNet MMI descriptions:
    // light 1-3, moderate 4-5, strong 6-7, major 8+
    public string Severity => Mmi switch
    {
        <= 3 => "light",
        <= 5 => "moderate",
        <= 7 => "strong",
        _ => "major",
    };
}
