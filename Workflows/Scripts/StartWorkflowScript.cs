using ScriptRunner;

namespace Workflows.Scripts
{
    public class StartWorkflowScript : CompiledScript
    {
        public StartWorkflowScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        public void StartWorkflow()
        {

        }
    }
}
