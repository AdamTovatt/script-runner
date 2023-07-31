using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using System.Text;

namespace Workflows.Scripts
{
    public class ExitWorkflowScript : CompiledScript
    {
        public ExitWorkflowScript(ScriptContext context) : base(context) { }

        [Summary("Will exit the workflow.")]
        [Parameter("exitMessage", "A message about the exit status and why the workflow exited.")]
        [Returns("A message about the exit status")]
        public string ExitWorklow(string exitMessage)
        {
            IWorkflowContext workflow = (IWorkflowContext)Context;

            StringBuilder message = new StringBuilder();

            message.AppendLine($"You are currently in a workflow that will {workflow.GetPurpose()}");
            message.AppendLine("A workflow is a collection of tasks that you will complete, if the task requires you to take input from the use you will do so. ");
            message.AppendLine("When you have aquired all neccessary inputs from the user and completed everything there is to do in the current task you will continue by calling the GoToNextStep function.");
            message.AppendLine($"If the users wants to exit the workflow prematurely they can. Maybe they change their mind and don't want to continue. You can help them by calling first warning them that if they exit the progress towards {workflow.GetPurpose()} will be lost, if they still want to exit you can call the ExitWorkflow() function with a short message about what happened.");

            if (workflow.GetSavedValues().Count > 0)
            {
                message.AppendLine("As a reminder, here are the values you have collected from the user so far: ");

                foreach (KeyValuePair<string, object> keyValuePair in workflow.GetSavedValues())
                    message.AppendLine($"{keyValuePair.Key}: {keyValuePair.Value}");
            }

            message.AppendLine();
            message.AppendLine("Now, here is your current task: ");
            message.AppendLine(workflow.GoToNextStep());

            return message.ToString();
        }
    }
}
