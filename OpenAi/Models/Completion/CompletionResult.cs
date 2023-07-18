namespace OpenAi.Models.Completion
{
    public class CompletionResult
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }

        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }

        public CompletionResult(string id, string @object, long created, List<Choice> choices, Usage usage)
        {
            Id = id;
            Object = @object;
            Created = created;
            Choices = choices;
            Usage = usage;
        }
    }
}
