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

        [JsonIgnore]
        public bool HasFunctionCalls { get { return Choices.Any(choice => choice.FinishReason == FinishReason.FunctionCall); } }

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

        public List<FunctionCall> GetFunctionCalls()
        {
            List<FunctionCall> functions = new List<FunctionCall>();

            foreach (Choice choice in Choices)
            {
                if (choice.FinishReason == FinishReason.FunctionCall)
                {
                    if (choice.Message.FunctionCall != null)
                        functions.Add(choice.Message.FunctionCall);
                }
            }

            return functions;
        }

        public List<Message> GetMessages()
        {
            List<Message> messages = new List<Message>();

            foreach (Choice choice in Choices)
            {
                if (choice.FinishReason != FinishReason.FunctionCall && choice.Message != null)
                    messages.Add(choice.Message);
            }

            return messages;
        }
    }
}
