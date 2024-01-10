using ScriptRunner.DocumentationAttributes;

namespace ScriptRunner.Workflows.Scripts
{
    public class ExitWorkflowScript : CompiledScript
    {
        public ExitWorkflowScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("Will exit the workflow.")]
        [Parameter("exitMessage", "A message about the exit status and why the workflow exited.")]
        [Returns("A message about the exit status")]
        public string ExitWorklow(string exitMessage)
        {
            return Context.Conversation.ExitWorkflow(exitMessage);
        }
    }
}
