using ScriptRunner.Workflows;

namespace ScriptRunner.Providers
{
    public interface IWorkflowProvider
    {
        public Task<Workflow?> GetWorkflowAsync(string workflowName);
        public Task<bool> SaveWorkflowAsync(Workflow workflow);
    }
}
