namespace OpenAi.Models.Completion
{
    public class CompletionParameter
    {
        public string Model { get; set; }
        public List<Message> Messages { get; set; }
        public List<Function>? Functions { get; set; }

        public CompletionParameter(string model, List<Message> messages, List<Function>? functions)
        {
            Model = model;
            Messages = messages;
            Functions = functions;
        }

        public CompletionParameter(string model)
        {
            Model = model;
            Messages = new List<Message>();
        }

        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }

        public void AddSystemMessage(string content)
        {
            Messages.Add(new Message(Role.System, content));
        }

        public void AddAssistantMessage(string content)
        {
            Messages.Add(new Message(Role.Assistant, content));
        }

        public void AddUserMessage(string content)
        {
            Messages.Add(new Message(Role.User, content));
        }

        public void AddFunction(Function function)
        {
            if(Functions == null) 
                Functions = new List<Function>();

            Functions.Add(function);
        }
    }
}
