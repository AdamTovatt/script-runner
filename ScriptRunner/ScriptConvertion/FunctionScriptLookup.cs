using ScriptRunner.Models;
using ScriptRunner.OpenAi.Models.Completion;
using ScriptRunner.Providers;
using System.Security.Claims;

namespace ScriptRunner.ScriptConvertion
{
    /// <summary>
    /// Used to contain both functions and script compile results
    /// </summary>
    public class FunctionScriptLookup
    {
        private Dictionary<string, ICompiledScriptContainer> scriptCompileResults;
        private List<Function> functions;
        private List<ICodeProvider> codeProviders;
        private List<ICompiledScriptProvider> compiledScriptProviders;
        private ClaimsPrincipal? claimsPrincipal;

        /// <summary>
        /// Will create a new instance of the FunctionScriptLookup class based on the given code providers
        /// </summary>
        /// <param name="codeAndScriptProviders">The code providers and script providers to create this lookup from</param>
        public FunctionScriptLookup(params ICodeOrScriptProvider[] codeAndScriptProviders)
        {
            scriptCompileResults = new Dictionary<string, ICompiledScriptContainer>();
            functions = new List<Function>();
            codeProviders = new List<ICodeProvider>();
            compiledScriptProviders = new List<ICompiledScriptProvider>();

            foreach (ICodeOrScriptProvider provider in codeAndScriptProviders)
            {
                if (provider is ICompiledScriptProvider scriptProvider)
                    compiledScriptProviders.Add(scriptProvider);
                else if(provider is ICodeProvider codeProvider)
                    codeProviders.Add(codeProvider);
                else
                    throw new NotSupportedException($"The given code or script provider ({provider.GetType().Name}) is not a valid code or script provider");
            }
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

                foreach(ICompiledScriptProvider compiledScriptProvider in compiledScriptProviders) // go through the precompiled scripts and add them
                {
                    foreach (ICompiledScriptContainer compiledScriptContainer in compiledScriptProvider.GetCompiledScripts())
                    {
                        if (compiledScriptContainer is ICompiledScriptContainer compiledScript)
                            functionsScriptMap.Add(OpenAiScriptConverter.GetAsFunction(compiledScript), compiledScript);
                        else
                            throw new Exception($"The compiled script container ({compiledScriptContainer.GetType().Name}) is not a valid compiled script container");
                    }
                }

                functions.Clear();
                scriptCompileResults.Clear();

                foreach (KeyValuePair<Function, ICompiledScriptContainer> mapPair in functionsScriptMap)
                {
                    Function function = mapPair.Key;

                    if(function.AllowedRoles != null)
                    {
                        if (claimsPrincipal == null)
                            throw new InvalidOperationException($"Claims principal as not been set, but the function ({function.Name}) requires a claims principal. Use .SetClaimsPrincipal() on the FunctionScriptLookup before loading functions that have specific allowed roles. ");

                        if (!IsInAllowedRoles(claimsPrincipal, function.AllowedRoles))
                            continue; // skip this function, since the user is not in any of the allowed roles
                    }

                    functions.Add(function);
                    scriptCompileResults.Add(function.Name, mapPair.Value);
                }

                return null;
            }
            catch (ConvertionException convertionException)
            {
                return convertionException.Errors;
            }
        }

        private bool IsInAllowedRoles(ClaimsPrincipal claimsPrincipal, IEnumerable<string> allowedRoles)
        {
            if (claimsPrincipal.Identity == null)
                return false;

            foreach (string role in allowedRoles)
            {
                if (((ClaimsIdentity)claimsPrincipal.Identity).Claims.Any(x => x.Type == "permissions" && x.Value == role))
                    return true;
            }

            return false;
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

        public void CombineWith(FunctionScriptLookup? other)
        {
            if (other == null) return;

            compiledScriptProviders.AddRange(other.compiledScriptProviders);
            codeProviders.AddRange(other.codeProviders);
        }

        public void SetClaimsPrincipal(ClaimsPrincipal? principal)
        {
            claimsPrincipal = principal;
        }
    }
}
