using QuakeWatch.Api.Models;

namespace QuakeWatch.Api.Tests;

public class QuakeSeverityTests
{
    // Keeps each test down to the value it cares about.
    private static Quake QuakeWithMmi(int mmi) => new Quake(
        PublicID: "test-id",
        Magnitude: 5.0,
        Depth: 10,
        Locality: "Test locality",
        Mmi: mmi,
        Quality: "best",
        Time: DateTimeOffset.UtcNow
    );

    // The rows sit on the band boundaries, where off-by-one bugs live.
    [Theory]
    [InlineData(-1, "light")]
    [InlineData(0, "light")]
    [InlineData(1, "light")]
    [InlineData(3, "light")]      // top of light
    [InlineData(4, "moderate")]   // bottom of moderate
    [InlineData(5, "moderate")]   // top of moderate
    [InlineData(6, "strong")]     // bottom of strong
    [InlineData(7, "strong")]     // top of strong
    [InlineData(8, "major")]      // bottom of major
    [InlineData(12, "major")]     // top of the MMI scale
    public void Severity_matches_the_expected_band(int mmi, string expected)
    {
        var quake = QuakeWithMmi(mmi);

        Assert.Equal(expected, quake.Severity);
    }

    // Every MMI value must land in a band, with no gaps.
    [Fact]
    public void Every_possible_mmi_gets_a_severity()
    {
        for (var mmi = -1; mmi <= 12; mmi++)
        {
            var severity = QuakeWithMmi(mmi).Severity;

            Assert.False(
                string.IsNullOrWhiteSpace(severity),
                $"MMI {mmi} produced no severity");
        }
    }

    [Fact]
    public void Severity_is_only_ever_one_of_four_words()
    {
        var allowed = new[] { "light", "moderate", "strong", "major" };

        for (var mmi = -1; mmi <= 12; mmi++)
        {
            Assert.Contains(QuakeWithMmi(mmi).Severity, allowed);
        }
    }
}
