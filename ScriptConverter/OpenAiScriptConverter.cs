using OpenAi.Models.Completion;
using OpenAi.Models.Completion.Parameters;
using ScriptRunner;
using ScriptRunner.Models;
using ScriptRunner.Providers;
using System.Reflection;
using System.Text;

namespace ScriptConverter
{
    public class OpenAiScriptConverter
    {
        /// <summary>
        /// Will get all the functions from a script provider
        /// </summary>
        /// <param name="scriptProvider">The script provider to use</param>
        /// <returns>All the functions that the provider has</returns>
        /// <exception cref="ConvertionException">Will throw a ConvertionException if any compilation errors occur. The error messages will be in the .Errors property of the exception object (List of string)</exception>
        public static async Task<Dictionary<Function, ScriptCompileResult>> GetAllFunctionsAsync(ICodeProvider scriptProvider)
        {
            List<string> errors = new List<string>();
            Dictionary<Function, ScriptCompileResult> result = new Dictionary<Function, ScriptCompileResult>();

            List<ScriptCode> scripts = await scriptProvider.GetAllScriptsAsync();
            List<Task> compileTasks = new List<Task>();

            foreach (ScriptCode script in scripts)
            {
                compileTasks.Add(Task.Run(() =>
                {
                    ScriptCompileResult compileResult = script.Compile();

                    if (compileResult.Errors != null)
                    {
                        lock (errors)
                        {
                            errors.Add($"{compileResult.GetErrorMessages()} for script: {script.Code}");
                        }
                    }

                    if (compileResult.CompiledAssembly != null)
                    {
                        lock (result)
                        {
                            result.Add(GetAsFunction(compileResult), compileResult);
                        }
                    }
                }));
            }

            await Task.WhenAll(compileTasks);

            if (errors.Count > 0)
                throw new ConvertionException(errors);

            return result;
        }

        public static Function GetAsFunction(ICompiledScriptContainer compiledScript)
        {
            Type? scriptType = compiledScript.GetScriptType();

            if (scriptType == null)
                throw new InvalidOperationException("The script doesn't contain a type that inherits from CompiledScript");

            MethodInfo? startMethod = scriptType.GetMethods().SingleOrDefault(method => method.GetCustomAttribute<ScriptStart>() != null);

            if (startMethod == null)
                throw new InvalidOperationException("The script doesn't contain a method with the ScriptStart attribute");

            ParameterInfo[] parameters = startMethod.GetParameters();

            ICommentProvider? comment = compiledScript.GetCommentProvider(startMethod);
            Function function = new Function($"{startMethod.Name}", comment?.Summary ?? "");

            foreach (ParameterInfo parameter in parameters)
            {
                if (parameter.Name == null) continue;

                string parameterName = parameter.Name;
                string? parameterDescription = comment?.GetParameterDescription(parameter.Name);
                function.Parameters.Add(new Parameter(parameterName, parameter.ParameterType, parameterDescription ?? ""), true);
            }

            return function;
        }
    }
}