using OpenAi.Models.Completion;
using OpenAi.Models.Completion.Parameters;
using ScriptRunner;
using ScriptRunner.Models;
using System.Reflection;

namespace ScriptConverter
{
    public class OpenAiScriptConverter
    {
        public static Function GetAsFunction(ScriptCompileResult compileResult)
        {
            Type? scriptType = compileResult.GetScriptType();

            if (scriptType == null)
                throw new InvalidOperationException("The script doesn't contain a type that inherits from CompiledScript");

            MethodInfo? startMethod = scriptType.GetMethods().SingleOrDefault(method => method.GetCustomAttribute<ScriptStart>() != null);

            if (startMethod == null)
                throw new InvalidOperationException("The script doesn't contain a method with the ScriptStart attribute");

            ParameterInfo[] parameters = startMethod.GetParameters();

            Function function = new Function($"{scriptType.Name}.{startMethod.Name}", description);

            foreach (ParameterInfo parameter in parameters)
            {
                function.Parameters.Add(new Parameter(parameter.Name, parameter.ParameterType, ""), true);
            }

            return function;
        }
    }
}