namespace OpenAi.Models.Completion
{
    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public string? Name { get; set; }
        public string? FunctionCall { get; set; }

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
    }
}
