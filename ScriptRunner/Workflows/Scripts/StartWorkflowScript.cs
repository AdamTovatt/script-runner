using ScriptRunner.DocumentationAttributes;

namespace ScriptRunner.Workflows.Scripts
{
    public class StartWorkflowScript : CompiledScript
    {
        public StartWorkflowScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("Will start a workflow with the given name.")]
        [Parameter("workflowName", "The name of the workflow to start.")]
        public async Task<string> StartWorkflow(string workflowName)
        {
            return await Context.Conversation.EnterWorkflowAsync(workflowName);
        }
    }
}
