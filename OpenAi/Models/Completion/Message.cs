namespace OpenAi.Models.Completion
{
    public class Message
    {
        public Role Role { get; set; }
        public string Content { get; set; }
        public string? Name { get; set; }
        public string? FunctionCall { get; set; }

        public Message(Role role, string content, string? name, string? functionCall)
        {
            Role = role;
            Content = content;
            Name = name;
            FunctionCall = functionCall;
        }
    }
}
