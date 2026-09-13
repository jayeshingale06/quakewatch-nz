namespace QuakeWatch.Api.Models.GeoNet;

// These records describe GEONET'S shape, not ours.
// We only need them long enough to read the response.
// Our own shape is Models/Quake.cs.

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
