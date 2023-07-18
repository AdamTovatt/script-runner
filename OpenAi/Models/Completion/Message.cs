namespace OpenAi.Models.Completion
{
    public class Message
    {
        public Role Role { get; set; }
        public string Content { get; set; }

        public Message(Role role, string content)
        {
            Role = role;
            Content = content;
        }
    }
}
