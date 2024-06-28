using System.Text.Json;
using System.Text.Json.Serialization;

namespace Klayman.Domain.JsonConverters;

public class KeyboardLayoutIdJsonConverter : JsonConverter<KeyboardLayoutId>
{
    public override KeyboardLayoutId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new KeyboardLayoutId(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, KeyboardLayoutId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}