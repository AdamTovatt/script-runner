using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using ScriptRunner.Workflows.Scripts;

namespace SkippyBackend.PrecompiledScripts
{
    public class CreateSingleItemScript : CompiledScript
    {
        public CreateSingleItemScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("Will create a single item cover for the user, this is a type of insurance. If the user wants to insure something, call this function. ")]
        [Parameter("item", "The item that the user wants to insure")]
        [Parameter("itemValue", "The value of the item to insure")]
        [Parameter("amountOfRecentClaims", "The amount of recent claims the user has")]
        public async Task<string> CreateSingleItem(string item, int itemValue, int amountOfRecentClaims)
        {
            StartWorkflowScript startWorkflowScript = new StartWorkflowScript(Context);
            return await startWorkflowScript.StartWorkflow("CreateSingleItemCoverForUser");

            //await Task.CompletedTask;
            //return "Ok";
        }
    }
}
