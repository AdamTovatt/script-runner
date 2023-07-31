using OpenAi.Models.Completion;
using ScriptRunner;
using ScriptRunner.DocumentationAttributes;

namespace ScriptRunner.Workflows.Scripts
{
    public class StartWorkflowScript : CompiledScript
    {
        public StartWorkflowScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("Will start a workflow with the given name.")]
        [Parameter("workflowName", "The name of the workflow to start.")]
        public string StartWorkflow(string workflowName)
        {
            if (!typeof(WorkflowContext).IsAssignableFrom(Context.GetType()))
                return $"A workflow can not be started because the current context ({Context.GetType()}) does not implement IWorkflowContext.";

            WorkflowContext workflowContext = (WorkflowContext)Context;
            return workflowContext.EnterWorkflow(workflowName);
        }
    }
}
