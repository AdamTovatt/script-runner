using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using ScriptRunner.OpenAi.Models.Completion;

namespace SkippyBackend.PrecompiledScripts
{
    public class GetAvailableFunctionsScript : CompiledScript
    {
        public GetAvailableFunctionsScript(ScriptContext context) : base(context) { }

        [Summary("Will return a list of the names for all available functions that have been loaded and are callable")]
        [ScriptStart]
        public string GetAvailableFunctions()
        {
            try
            {
                if (Context.Conversation.FunctionLookup == null)
                    throw new Exception("There are no functions");

                List<Function> functions = Context.Conversation.FunctionLookup.GetFunctions().OrderBy(x => x.Name).ToList();

                List<string> result = new List<string>();

                functions.ForEach(function => result.Add(function.Name));

                return string.Join(", ", result);
            }
            catch (Exception exception)
            {
                return $"An error occured while getting the available functions: {exception.Message}";
            }
        }
    }
}