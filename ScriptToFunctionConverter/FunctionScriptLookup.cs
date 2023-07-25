using OpenAi.Models.Completion;
using ScriptConverter;
using ScriptRunner.Models;
using ScriptRunner.Providers;

namespace ScriptConverter
{
    /// <summary>
    /// Used to contain both functions and script compile results
    /// </summary>
    public class FunctionScriptLookup
    {
        private Dictionary<string, ScriptCompileResult> scriptCompileResults;
        private List<Function> functions;
        private ICodeProvider codeProvider;

        /// <summary>
        /// Will create a new instance of the FunctionScriptLookup class based on a code provider
        /// </summary>
        /// <param name="codeProvider"></param>
        public FunctionScriptLookup(ICodeProvider codeProvider)
        {
            scriptCompileResults = new Dictionary<string, ScriptCompileResult>();
            functions = new List<Function>();
            this.codeProvider = codeProvider;
        }

        /// <summary>
        /// Will load all the functions from the code provider
        /// </summary>
        /// <returns>Will return a list of errors, or null if there were no errors</returns>
        public async Task<List<string>?> LoadFunctionsAsync()
        {
            try
            {
                functions.Clear();
                scriptCompileResults.Clear();

                Dictionary<Function, ScriptCompileResult> openAiScriptConverter = await OpenAiScriptConverter.GetAllFunctionsAsync(codeProvider);

                foreach (KeyValuePair<Function, ScriptCompileResult> function in openAiScriptConverter)
                {
                    functions.Add(function.Key);
                    scriptCompileResults.Add(function.Key.Name, function.Value);
                }

                return null;
            }
            catch (ConvertionException convertionException)
            {
                return convertionException.Errors;
            }
        }

        /// <summary>
        /// Will try to get a compile result
        /// </summary>
        /// <param name="functionName">The function name to try to find the compile result for</param>
        /// <param name="scriptCompileResult">The resulting SCriptCompileResult as an out parameter, if there was any</param>
        /// <returns>Wether or not it found a ScriptCompileResult</returns>
        public bool TryGetCompileResult(string functionName, out ScriptCompileResult scriptCompileResult)
        {
            bool result = scriptCompileResults.TryGetValue(functionName, out ScriptCompileResult? compileResult);

            if (compileResult == null)
                throw new Exception($"Tried to get compile result from function name ({functionName}) but the compile result was null, this should not happen");

            scriptCompileResult = compileResult;
            return result;
        }

        /// <summary>
        /// Will return the functions in this lookup
        /// </summary>
        /// <returns>The functions in this lookup</returns>
        public List<Function> GetFunctions()
        {
            return functions;
        }
    }
}
