using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using ScriptRunner.Workflows;

namespace SkippyBackend.PrecompiledScripts
{
    public class CreateWorkflowScript : CompiledScript
    {
        public CreateWorkflowScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("Will create a workflow from the given parameters")]
        [Parameter("name", "The name of the workflow")]
        [Parameter("purpose", "The purpose of the workflow. Should fit in the sentence: this is a workflow that will ...")]
        [Parameter("tasks", "A list of task descriptions. Each task description is a description of a task and what input is required for that task")]
        public async Task<string> CreateWorkflow(string name, string purpose, string[] tasks)
        {
            if (Context.Conversation.WorkflowProvider == null) return "Error: No workflow can be created and saved since no workflowprovider has been specified for this conversation";

            Workflow workflow = new Workflow(name, tasks.ToList(), purpose);
            bool result = await Context.Conversation.WorkflowProvider.SaveWorkflowAsync(workflow);

            if (result)
                return $"Workflow {name} was created and saved successfully";
            else
                return $"Workflow {name} was not created, there was some unknown error";
        }
    }
}
