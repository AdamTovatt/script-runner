using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Converters
{
    public class ArgumentsConverter : JsonConverter<Dictionary<string, JsonNode>?>
    {
        public override Dictionary<string, JsonNode>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? json = reader.GetString();

            if (json == null)
                return null;
            if (json == "{}")
                return null;

            JsonSerializerOptions stringChunkOptions = new JsonSerializerOptions() { UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode };
            return JsonSerializer.Deserialize<Dictionary<string, JsonNode>>(json, stringChunkOptions);
        }
        //"{\n  \"factor1\": 2,\n  \"factor2\": 2\n}"
        public override void Write(Utf8JsonWriter writer, Dictionary<string, JsonNode>? value, JsonSerializerOptions options)
        {
            string json = $"\"{JsonSerializer.Serialize(value).Replace("\"", "\\\"")}\"";
            writer.WriteRawValue(json);
        }
    }
}
