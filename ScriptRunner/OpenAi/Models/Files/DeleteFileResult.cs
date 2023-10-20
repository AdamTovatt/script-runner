using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Files
{
    public class DeleteFileResult : OpenAiApiResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        public DeleteFileResult(string @object, string id, bool deleted)
        {
            Object = @object;
            Id = id;
            Deleted = deleted;
        }

        public static DeleteFileResult FromJson(string json)
        {
            DeleteFileResult? result = JsonSerializer.Deserialize<DeleteFileResult>(json);

            if (result == null) throw new JsonException($"Could not deserialize typeof @{typeof(DeleteFileResult)} from json with lengt {json.Length}: {json}");
            return result;
        }
    }
}
