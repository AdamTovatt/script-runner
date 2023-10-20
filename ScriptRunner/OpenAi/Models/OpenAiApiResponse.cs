using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models
{
    public abstract class OpenAiApiResponse
    {
        [JsonPropertyName("error")]
        public ErrorResponse? Error { get; set; }
    }
}
