namespace OpenAi.Models.Completion
{
    public class CompletionParameter
    {
        public Model Model { get; set; }
        public List<Message> Messages { get; set; }
        public List<Function> Functions { get; set; }

        public CompletionParameter(Model model, List<Message> messages, List<Function> functions)
        {
            Model = model;
            Messages = messages;
            Functions = functions;
        }
    }
}
