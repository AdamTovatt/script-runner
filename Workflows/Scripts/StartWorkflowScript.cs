using OpenAi.Models.Completion;
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
            if (!typeof(IWorkflowContext).IsAssignableFrom(Context.GetType()))
                return $"A workflow can not be started because the current context ({Context.GetType()}) does not implement IWorkflowContext.";

            IWorkflowContext workflowContext = (IWorkflowContext)Context;
            return workflowContext.StartWorkflow(workflowName);
        }
    }
}
