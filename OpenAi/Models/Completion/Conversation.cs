namespace OpenAi.Models.Completion
{
    public class Conversation
    {
        public List<Message> Messages { get; set; }
        public List<Function>? Functions { get; set; }

        public Conversation()
        {
            Messages = new List<Message>();
        }

        public void Add(CompletionParameter parameter)
        {
            if (parameter.Messages == Messages)
                return;

            foreach (Message message in parameter.Messages)
            {
                Messages.Add(message);
            }
        }

        public void Add(CompletionResult result)
        {
            foreach (Choice choice in result.Choices)
            {
                if (choice.FinishReason != FinishReason.FunctionCall)
                    Messages.Add(choice.Message);
                else
                {
                    Messages.Add(new Message(Role.Assistant, "(Calling function)"));
                }
            }
        }

        public void Add(Function function)
        {
            if (Functions == null)
                Functions = new List<Function>();

            Functions.Add(function);
        }

        public CompletionParameter CreateCompletionParameter(string model)
        {
            return new CompletionParameter(model, Messages, Functions);
        }
    }
}
