using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppointmentSystem.Utils.Converters
{
    public class DateOnlyJSONConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            DateOnly value = DateOnly.FromDateTime(reader.GetDateTime());
            return value;
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            var isoDate = value.ToString("O");
            writer.WriteStringValue(isoDate);
        }
    }
}
