using ScriptRunner;

namespace SkippyBackend.Models
{
    public class SkippyContext : ScriptContext
    {
        public ClientData ClientData { get; set; }

        public SkippyContext(ClientData clientData) : base(clientData.Conversation)
        {
            ClientData = clientData;
        }
    }
}
