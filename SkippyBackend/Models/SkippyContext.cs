using ScriptRunner.ScriptConvertion;
using ScriptRunner.Workflows;

namespace SkippyBackend.Models
{
    public class SkippyContext : WorkflowContext
    {
        public FunctionScriptLookup ScriptLookup { get; set; }
        public ClientData ClientData { get; set; }

        public SkippyContext(FunctionScriptLookup scriptLookup, ClientData clientData) : base(clientData.Conversation)
        {
            ScriptLookup = scriptLookup;
            ClientData = clientData;
        }
    }
}
