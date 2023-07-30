using OpenAi.Models.Completion;
using SkippyBackend.FrontEndModels;

namespace SkippyBackend.Models
{
    public class ClientData
    {
        public ChatConfiguration ChatConfiguration { get; set; }
        public Conversation Conversation { get; set; }

        public ClientData(ChatConfiguration chatConfiguration, Conversation conversation)
        {
            ChatConfiguration = chatConfiguration;
            Conversation = conversation;
        }
    }
}
