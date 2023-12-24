using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models
{
    public abstract class OpenAiApiResponse
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonPropertyName("error")]
        public ErrorResponse? Error { get; set; }
    }
}
