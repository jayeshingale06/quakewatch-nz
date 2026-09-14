namespace QuakeWatch.Api.Models;

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
    // Bands follow GeoNet's MMI descriptions: light 1-3, moderate 4-5,
    // strong 6-7, major 8 and above.
    public string Severity => Mmi switch
    {
        <= 3 => "light",
        <= 5 => "moderate",
        <= 7 => "strong",
        _ => "major",
    };
}
