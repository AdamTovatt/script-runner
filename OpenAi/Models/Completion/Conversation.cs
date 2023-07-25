using OpenAi.Helpers;

namespace OpenAi.Models.Completion
{
    public class Conversation
    {
        public List<Message> Messages { get; set; }
        public List<Function>? Functions { get; set; }
        public string Model { get; set; }
        public int? TokenLimit { get; set; }

        private TokenCounter tokenCounter;

        public Conversation(string model, int? tokenLimit)
        {
            TokenLimit = tokenLimit;
            Model = model;
            Messages = new List<Message>();
            tokenCounter = new TokenCounter(model);
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

            if (TokenLimit != null && TokenLimit > 0)
            {
                while (tokenCounter.GetTokenCount(this) > TokenLimit)
                {
                    Messages.RemoveAt(0);
                }
            }
        }

        public void Add(Function function)
        {
            if (Functions == null)
                Functions = new List<Function>();

            Functions.Add(function);
        }

        public CompletionParameter CreateCompletionParameter()
        {
            return new CompletionParameter(Model, Messages, Functions);
        }
    }
}
