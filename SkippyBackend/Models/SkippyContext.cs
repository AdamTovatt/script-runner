using ScriptConverter;
using Workflows;

namespace SkippyBackend.Models
{
    public class SkippyContext : WorkflowContext
    {
        public FunctionScriptLookup ScriptLookup { get; set; }
        public ClientData ClientData { get; set; }

        public SkippyContext(FunctionScriptLookup scriptLookup, ClientData clientData) : base(clientData.Conversation, null, null)
        {
            ScriptLookup = scriptLookup;
            ClientData = clientData;
        }
    }
}
