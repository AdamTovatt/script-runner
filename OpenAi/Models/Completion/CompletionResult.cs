using System.Text.Json.Serialization;

namespace OpenAi.Models.Completion
{
    public class CompletionResult
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }

        public List<Choice> Choices { get; set; }
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
    }
}
