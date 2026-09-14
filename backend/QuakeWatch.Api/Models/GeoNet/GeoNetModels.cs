namespace QuakeWatch.Api.Models.GeoNet;

// GeoNet's response shape. Ours is Models/Quake.cs.
public record GeoNetResponse(
    List<GeoNetFeature>? Features
);

public record GeoNetFeature(
    GeoNetProperties? Properties
);

public record GeoNetProperties(
    string? PublicID,
    DateTimeOffset Time,
    double Depth,
    double Magnitude,
    int Mmi,
    string? Locality,
    string? Quality
);
