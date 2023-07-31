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
        private Dictionary<string, ICompiledScriptContainer> scriptCompileResults;
        private List<Function> functions;
        private List<ICodeProvider> codeProviders;

        /// <summary>
        /// Will create a new instance of the FunctionScriptLookup class based on the given code providers
        /// </summary>
        /// <param name="codeProviders">The code providers to create this lookup from</param>
        public FunctionScriptLookup(params ICodeProvider[] codeProviders)
        {
            scriptCompileResults = new Dictionary<string, ICompiledScriptContainer>();
            functions = new List<Function>();
            this.codeProviders = new List<ICodeProvider>();

            foreach (ICodeProvider codeProvider in codeProviders)
                this.codeProviders.Add(codeProvider);
        }

        /// <summary>
        /// Will load all the functions from the code provider
        /// </summary>
        /// <returns>Will return a list of errors, or null if there were no errors</returns>
        public async Task<List<string>?> LoadFunctionsAsync()
        {
            try
            {
                Dictionary<Function, ICompiledScriptContainer> functionsScriptMap = new Dictionary<Function, ICompiledScriptContainer>();

                foreach(ICodeProvider codeProvider in codeProviders) // go through all the code providers and compile their codes
                {
                    Dictionary<Function, ScriptCompileResult> openAiScriptConverter = await OpenAiScriptConverter.GetAllFunctionsAsync(codeProvider);

                    foreach (KeyValuePair<Function, ScriptCompileResult> function in openAiScriptConverter)
                        functionsScriptMap.Add(function.Key, function.Value); // add all the compile results
                }

                functions.Clear();
                scriptCompileResults.Clear();

                foreach (KeyValuePair<Function, ICompiledScriptContainer> mapPair in functionsScriptMap)
                {
                    functions.Add(mapPair.Key);
                    scriptCompileResults.Add(mapPair.Key.Name, mapPair.Value);
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
        public bool TryGetCompiledScriptContainer(string functionName, out ICompiledScriptContainer scriptCompileResult)
        {
            bool result = scriptCompileResults.TryGetValue(functionName, out ICompiledScriptContainer? compiledScriptContainer);

            if (compiledScriptContainer == null)
                throw new Exception($"Tried to get compile result from function name ({functionName}) but the compile result was null, this should not happen");

            scriptCompileResult = compiledScriptContainer;
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
