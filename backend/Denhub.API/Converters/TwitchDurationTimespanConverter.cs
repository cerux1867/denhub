using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Denhub.API.Converters {
    public class TwitchDurationTimespanConverter : JsonConverter<TimeSpan> {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var durationString = reader.GetString();
            if (string.IsNullOrEmpty(durationString)) {
                return TimeSpan.Zero;
            }

            var hoursIndex = FindIndexOf(durationString, "h");
            var durationHours = ExtractNumber(durationString, hoursIndex);
            var minutesIndex = FindIndexOf(durationString, "m");
            var durationMinutes = ExtractNumber(durationString, minutesIndex, hoursIndex);
            var secondsIndex = FindIndexOf(durationString, "s");
            var durationSeconds = ExtractNumber(durationString, secondsIndex, minutesIndex);

            return new TimeSpan(durationHours, durationMinutes, durationSeconds);
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

        private int FindIndexOf(string durationString, string timeUnit) {
            return durationString.IndexOf(timeUnit, StringComparison.InvariantCulture);
        }

        private int ExtractNumber(string durationString, int unitIndex, int offsetIndex = -1) {
            var index = unitIndex;
            var durationNumber = 0;
            if (index <= -1) return durationNumber;
            var difference = index - (offsetIndex + 1);
            durationNumber = Convert.ToInt32(durationString.Substring(offsetIndex + 1, difference));

            return durationNumber;
        }
    }
}