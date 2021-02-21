using System.Text;
using System.Text.Json;
using Denhub.API.Converters;
using Xunit;

namespace Denhub.API.Tests.Converters {
    public class TwitchDurationTimespanConverterTests {
        [Theory]
        [InlineData(10, 15, 33)]
        [InlineData(100, 59, 59)]
        [InlineData(1, 1, 1)]
        [InlineData(0, 0, 13)]
        public void Read_VariousDurations_ParsedTimeSpan(int hours, int minutes, int seconds) {
            var durationString = "";
            if (hours > 0) {
                durationString += $"{hours}h";
            }

            if (minutes > 0) {
                durationString += $"{minutes}m";
            }

            if (seconds > 0) {
                durationString += $"{seconds}s";
            }
            var json = "{ \"duration\": \"" + durationString + "\" }";
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json), false,
                new JsonReaderState(new JsonReaderOptions()));
            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.String) {
                    break;
                }
            }
            var converter = new TwitchDurationTimespanConverter();

            var parsed = converter.Read(ref reader, typeof(TwitchDurationTimespanConverter),
                new JsonSerializerOptions());
            
            Assert.Equal(hours * 60 * 60 + minutes * 60 + seconds, parsed.TotalSeconds);
        }
    }
}