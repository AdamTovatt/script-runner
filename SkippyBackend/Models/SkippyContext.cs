using ScriptConverter;
using ScriptRunner;

namespace SkippyBackend.Models
{
    public class SkippyContext : ScriptContext
    {
        public FunctionScriptLookup ScriptLookup { get; set; }
        public ClientData ClientData { get; set; }

        public SkippyContext(FunctionScriptLookup scriptLookup, ClientData clientData) : base()
        {
            ScriptLookup = scriptLookup;
            ClientData = clientData;
        }
    }
}
