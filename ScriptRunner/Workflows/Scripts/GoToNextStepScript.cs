using ScriptRunner.DocumentationAttributes;

namespace ScriptRunner.Workflows.Scripts
{
    public class GoToNextStepScript : CompiledScript
    {
        public GoToNextStepScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("Will go to the next step in the workflow. Should be called when a step in a workflow is completed as in all tasks in that step are completed or all the user inputs required for that step have been aquired.")]
        [Returns("The next step in the workflow.")]
        public string GoToNextStep()
        {
            if(Context.Conversation.Workflow == null)
                return "You are not currently in a workflow. You can start a workflow by calling the StartWorkflow() function.";

            return Context.Conversation.Workflow.GoToNextStep();
        }
    }
}
