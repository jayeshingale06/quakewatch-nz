using QuakeWatch.Api.Models;

namespace QuakeWatch.Api.Tests;

public class QuakeSeverityTests
{
    // A helper so each test only says what it cares about: the MMI.
    private static Quake QuakeWithMmi(int mmi) => new Quake(
        PublicID: "test-id",
        Magnitude: 5.0,
        Depth: 10,
        Locality: "Test locality",
        Mmi: mmi,
        Quality: "best",
        Time: DateTimeOffset.UtcNow
    );

    // One test method, run once per row. Each row is a case.
    // The rows deliberately sit ON the band boundaries, because
    // boundaries are where off-by-one bugs live.
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

    // This is the test that proves the bug from Module 1 is fixed:
    // there is no MMI value that falls through with no answer.
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
