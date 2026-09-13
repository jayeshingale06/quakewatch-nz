// ARCHIVED from Module 3 Lesson 2.
// This was the hand written data, replaced by real GeoNet data in Lesson 3.
// Kept for revision. Not part of the build.

using QuakeWatch.Api.Models;

namespace QuakeWatch.Api.Data;

public static class SampleQuakes
{
    public static readonly List<Quake> All = new()
    {
        new Quake(
            PublicID: "2026p123456",
            Magnitude: 4.3,
            Depth: 12,
            Locality: "25 km south-east of Blenheim",
            Mmi: 4,
            Quality: "best",
            Time: DateTimeOffset.Parse("2026-09-11T21:14:03Z")
        ),
        new Quake(
            PublicID: "2026p123457",
            Magnitude: 5.8,
            Depth: 35,
            Locality: "15 km north-west of Christchurch",
            Mmi: 5,
            Quality: "best",
            Time: DateTimeOffset.Parse("2026-09-11T18:02:47Z")
        ),
        new Quake(
            PublicID: "2026p123458",
            Magnitude: 3.1,
            Depth: 5,
            Locality: "10 km north of Dunedin",
            Mmi: 2,
            Quality: "automatic",
            Time: DateTimeOffset.Parse("2026-09-11T16:30:00Z")
        ),
        new Quake(
            PublicID: "2026p123459",
            Magnitude: 6.2,
            Depth: 15,
            Locality: "30 km south-west of Kaikōura",
            Mmi: 7,
            Quality: "best",
            Time: DateTimeOffset.Parse("2026-09-10T09:45:22Z")
        ),
        new Quake(
            PublicID: "2026p123460",
            Magnitude: 7.1,
            Depth: 20,
            Locality: "40 km east of Wellington",
            Mmi: 9,
            Quality: "best",
            Time: DateTimeOffset.Parse("2026-09-12T10:30:00Z")
        ),
    };
}
