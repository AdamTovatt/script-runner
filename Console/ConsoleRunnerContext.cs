using ScriptConverter;
using ScriptRunner;

namespace Console
{
    public class ConsoleRunnerContext : ScriptContext
    {
        public FunctionScriptLookup FunctionScriptLookup { get; set; }

        public ConsoleRunnerContext(FunctionScriptLookup functionScriptLookup) : base()
        {
            FunctionScriptLookup = functionScriptLookup;
        }
    }
}
