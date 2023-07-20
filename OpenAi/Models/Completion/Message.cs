using System.Text.Json.Serialization;

namespace OpenAi.Models.Completion
{
    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("function_call")]
        public string? FunctionCall { get; set; }

        [JsonConstructor]
        public Message(string role, string content, string? name, string? functionCall)
        {
            Role = role;
            Content = content;
            Name = name;
            FunctionCall = functionCall;
        }

        public Message(string role, string content)
        {
            Role = role;
            Content = content;
        }

        public override string ToString()
        {
            if(FunctionCall == null)
                return $"{Role}: {Content}";
            return $"{Role}: (function) {FunctionCall}";
        }
    }
}
