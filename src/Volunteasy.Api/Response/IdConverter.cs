using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Volunteasy.Api.Response;

public class IdConverter : JsonConverter<long>
{
    public override long Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String) return reader.GetInt64();
        
        // try to parse number directly from bytes
        var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
        if (Utf8Parser.TryParse(span, out long number, out var bytesConsumed) && span.Length == bytesConsumed)
            return number;

        // try to parse from a string if the above failed,
        // this covers cases with other escaped/UTF characters
        return long.TryParse(reader.GetString(), out number) ? number : 
            reader.GetInt64();
    }

    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}