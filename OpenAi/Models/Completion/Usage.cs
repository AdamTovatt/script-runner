using System.Text.Json.Serialization;

namespace OpenAi.Models.Completion
{
    public class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }

        public override string ToString()
        {
            return $"Total tokens: {TotalTokens}";
        }
    }
}
