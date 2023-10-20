using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Files
{
    public class UploadFileResult : OpenAiApiResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("purpose")]
        public string Purpose { get; set; }
        [JsonPropertyName("filename")]
        public string FileName { get; set; }
        [JsonPropertyName("bytes")]
        public int Bytes { get; set; }
        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("status_details")]
        public string? StatusDetails { get; set; }

        public UploadFileResult(string @object, string id, string purpose, string fileName, int bytes, long createdAt, string status, string? statusDetails)
        {
            Object = @object;
            Id = id;
            Purpose = purpose;
            FileName = fileName;
            Bytes = bytes;
            CreatedAt = createdAt;
            Status = status;
            StatusDetails = statusDetails;
        }

        public static UploadFileResult FromJson(string json)
        {
            UploadFileResult? result = JsonSerializer.Deserialize<UploadFileResult>(json);

            if (result == null) throw new JsonException($"Could not deserialize typeof @{typeof(UploadFileResult)} from json with lengt {json.Length}: {json}");
            return result;
        }
    }
}
