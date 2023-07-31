using OpenAi.Models.Completion;
using ScriptRunner;
using ScriptRunner.Models;
using ScriptRunner.OpenAi.Models.Completion;
using ScriptRunner.Providers;
using SkippyBackend.Models;
using System;

namespace SkippyBackend.PrecompiledScripts
{
    public class GetAvailableFunctionsScript : CompiledScript
    {
        public GetAvailableFunctionsScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// Will return a list of the names for all available functions that have been loaded and are callable
        /// </summary>
        [ScriptStart]
        public string GetAvailableFunctions()
        {
            try
            {
                SkippyContext context = (SkippyContext)Context;

                List<Function> functions = context.ScriptLookup.GetFunctions();

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