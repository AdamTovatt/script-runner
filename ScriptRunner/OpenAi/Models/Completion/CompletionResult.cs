using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Completion
{
    public class CompletionResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }
        
        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }

        [JsonConstructor]
        public CompletionResult(string id, string @object, long created, string model, List<Choice> choices, Usage usage)
        {
            Id = id;
            Object = @object;
            Created = created;
            Model = model;
            Choices = choices;
            Usage = usage;
        }

        public override string ToString()
        {
            return $"Id: {Id}, Object: {Object}, Created: {Created}, Model: {Model}, Choices: {Choices}, Usage: {Usage}";
        }
    }
}
