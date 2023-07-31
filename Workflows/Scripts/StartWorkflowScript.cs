using ScriptRunner;
using ScriptRunner.DocumentationAttributes;

namespace Workflows.Scripts
{
    public class StartWorkflowScript : CompiledScript
    {
        public StartWorkflowScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("Will start a workflow with the given name.")]
        [Parameter("workflowName", "The name of the workflow to start.")]
        public string StartWorkflow(string workflowName)
        {
            return $"The workflow {workflowName} was started";
        }
    }
}
