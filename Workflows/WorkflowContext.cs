using OpenAi.Models.Completion;
using ScriptRunner;

namespace Workflows
{
    public class WorkflowContext : ScriptContext, IWorkflowContext
    {
        private int currentStep = 1;
        private List<string>? steps;
        private Dictionary<string, object> savedValues;
        private string? purpose;
        private bool isRunningWorkflow = false;

        public Conversation Conversation { get; set; }

        public WorkflowContext(Conversation conversation) : base()
        {
            Conversation = conversation;
            savedValues = new Dictionary<string, object>();
        }

        public Dictionary<string, object> GetSavedValues()
        {
            return savedValues;
        }

        public string GoToNextStep()
        {
            if(steps == null)
                return "Error - no steps were defined for this workflow";

            currentStep++;

            if (currentStep > steps.Count)
                return "Workflow completed, call the function to exit the work flow with a message about how the workflow went";

            return steps[currentStep - 1].ToString();
        }

        public void SaveValue(string key, object value)
        {
            if (savedValues.ContainsKey(key))
                savedValues[key] = value;
            else
                savedValues.Add(key, value);
        }

        public string GetPurpose()
        {
            return purpose;
        }

        public string StartWorkflow(string workflowName)
        {
            Conversation.ChildConversation = new Conversation(Conversation.Model, Conversation.TokenLimit);
            Conversation.ChildConversation.AddSystemMessage("Workflow mode was entered");

            Conversation.Functions

            return $"The workflow {workflowName} was started.";
        }
    }
}
