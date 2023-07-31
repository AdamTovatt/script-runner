using OpenAi.Models.Completion;

namespace Workflows
{
    public interface IWorkflowContext
    {
        public string GoToNextStep();
        public void SaveValue(string key, object value);
        public Dictionary<string, object> GetSavedValues();
        public string GetPurpose();
        public Conversation Conversation { get; }
        public string StartWorkflow(string workflowName);
    }
}
