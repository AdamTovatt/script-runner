using ScriptRunner.Helpers;
using ScriptRunner.Workflows;

namespace ScriptRunner.Providers
{
    public class DirectoryWorkflowProvider : IWorkflowProvider
    {
        public string DirectoryPath { get; set; }
        public string WorkflowFileExtension { get; set; } = ".json";

        public DirectoryWorkflowProvider(string directoryPath)
        {
            DirectoryPath = directoryPath;

            if (string.IsNullOrEmpty(directoryPath))
                DirectoryPath = Directory.GetCurrentDirectory();
        }

        public async Task<Workflow?> GetWorkflowAsync(string workflowName)
        {
            if (DirectoryPath.Length != 0 && !Directory.Exists(DirectoryPath))
                return null;

            string workflowPath;
            if (!workflowName.EndsWith(WorkflowFileExtension))
                workflowPath = $"{Path.Combine(DirectoryPath, workflowName)}{WorkflowFileExtension}";
            else
                workflowPath = $"{Path.Combine(DirectoryPath, workflowName)}";

            if (!File.Exists(workflowPath))
                return null;

            return Workflow.FromJson(await File.ReadAllTextAsync(workflowPath));
        }

        public async Task<bool> SaveWorkflowAsync(Workflow workflow)
        {
            string fileName = $"{workflow.Name}{WorkflowFileExtension}";
            string filePath = Path.Combine(DirectoryPath, fileName);

            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);

            await File.WriteAllTextAsync(filePath, workflow.ToJson());

            return true;
        }

        public DirectoryWorkflowProvider CreateFromRelativePath(string relativePath)
        {
            return new DirectoryWorkflowProvider(PathHelper.GetPathFromRelativePath(relativePath));
        }
    }
}
