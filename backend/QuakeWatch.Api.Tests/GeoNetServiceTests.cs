using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using QuakeWatch.Api.Services;

namespace QuakeWatch.Api.Tests;

public class GeoNetServiceTests
{
    // A FAKE network. It never leaves the machine. It just hands
    // back whatever JSON the test gave it.
    //
    // This is only possible because GeoNetService accepts an
    // HttpClient instead of creating one. That is dependency
    // injection paying for itself.
    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly string _json;
        private readonly HttpStatusCode _status;

        public StubHandler(string json, HttpStatusCode status = HttpStatusCode.OK)
        {
            _json = json;
            _status = status;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_status)
            {
                Content = new StringContent(_json, Encoding.UTF8, "application/json"),
            };

            return Task.FromResult(response);
        }
    }

    private static GeoNetService ServiceReturning(
        string json,
        HttpStatusCode status = HttpStatusCode.OK)
    {
        var http = new HttpClient(new StubHandler(json, status))
        {
            BaseAddress = new Uri("https://geonet.test/"),
        };

        return new GeoNetService(http, NullLogger<GeoNetService>.Instance);
    }

    // Real GeoNet shape, including their very long magnitudes.
    private const string TwoQuakesJson = """
    {
      "type": "FeatureCollection",
      "features": [
        {
          "properties": {
            "publicID": "older-quake",
            "time": "2026-09-11T04:25:03.646Z",
            "depth": 5,
            "magnitude": 3.9787732120687074,
            "mmi": 4,
            "locality": "10 km south of Taumarunui",
            "quality": "best"
          }
        },
        {
          "properties": {
            "publicID": "newer-quake",
            "time": "2026-09-12T04:25:03.646Z",
            "depth": 12.4499,
            "magnitude": 6.2512,
            "mmi": 7,
            "locality": "20 km east of Seddon",
            "quality": "automatic"
          }
        }
      ]
    }
    """;

    [Fact]
    public async Task Reads_both_quakes()
    {
        var service = ServiceReturning(TwoQuakesJson);

        var quakes = await service.GetQuakesAsync(0);

        Assert.Equal(2, quakes.Count);
    }

    [Fact]
    public async Task Rounds_magnitude_to_one_decimal_place()
    {
        var service = ServiceReturning(TwoQuakesJson);

        var quakes = await service.GetQuakesAsync(0);
        var quake = quakes.Single(q => q.PublicID == "older-quake");

        // GeoNet sent 3.9787732120687074
        Assert.Equal(4.0, quake.Magnitude);
    }

    [Fact]
    public async Task Rounds_depth_to_one_decimal_place()
    {
        var service = ServiceReturning(TwoQuakesJson);

        var quakes = await service.GetQuakesAsync(0);
        var quake = quakes.Single(q => q.PublicID == "newer-quake");

        // GeoNet sent 12.4499
        Assert.Equal(12.4, quake.Depth);
    }

    [Fact]
    public async Task Returns_newest_quake_first()
    {
        var service = ServiceReturning(TwoQuakesJson);

        var quakes = await service.GetQuakesAsync(0);

        Assert.Equal("newer-quake", quakes[0].PublicID);
        Assert.Equal("older-quake", quakes[1].PublicID);
    }

    [Fact]
    public async Task Works_out_severity_from_mmi()
    {
        var service = ServiceReturning(TwoQuakesJson);

        var quakes = await service.GetQuakesAsync(0);

        Assert.Equal("moderate", quakes.Single(q => q.Mmi == 4).Severity);
        Assert.Equal("strong", quakes.Single(q => q.Mmi == 7).Severity);
    }

    [Fact]
    public async Task Survives_a_response_with_no_features()
    {
        var service = ServiceReturning("""{ "type": "FeatureCollection", "features": [] }""");

        var quakes = await service.GetQuakesAsync(0);

        Assert.Empty(quakes);
    }

    [Fact]
    public async Task Fills_in_a_placeholder_when_locality_is_missing()
    {
        var json = """
        {
          "features": [
            {
              "properties": {
                "publicID": "no-locality",
                "time": "2026-09-12T04:25:03.646Z",
                "depth": 5,
                "magnitude": 4.0,
                "mmi": 4,
                "quality": "best"
              }
            }
          ]
        }
        """;

        var service = ServiceReturning(json);

        var quakes = await service.GetQuakesAsync(0);

        Assert.Equal("Unknown location", quakes[0].Locality);
    }

    [Fact]
    public async Task Throws_when_geonet_returns_an_error()
    {
        var service = ServiceReturning("server exploded", HttpStatusCode.ServiceUnavailable);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetQuakesAsync(0));
    }

    [Fact]
    public async Task Matches_field_names_whatever_their_capitalisation()
    {
        // GeoNet could send MMI, Mmi or mmi. All must work.
        var json = """
        {
          "features": [
            {
              "properties": {
                "PublicID": "shouty",
                "TIME": "2026-09-12T04:25:03.646Z",
                "Depth": 5,
                "MAGNITUDE": 4.0,
                "MMI": 7,
                "Locality": "Somewhere",
                "Quality": "best"
              }
            }
          ]
        }
        """;

        var service = ServiceReturning(json);

        var quakes = await service.GetQuakesAsync(0);

        Assert.Equal("shouty", quakes[0].PublicID);
        Assert.Equal(7, quakes[0].Mmi);
        Assert.Equal("strong", quakes[0].Severity);
    }
}
