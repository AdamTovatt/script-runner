using System.Reflection;
using System.Text.Json.Nodes;

namespace ScriptRunner
{
    public class CompiledScript
    {
        public ScriptContext Context { get; set; }

        public CompiledScript(ScriptContext context)
        {
            Context = context;
        }

        public object? Run(Dictionary<string, JsonNode>? parameters)
        {
            MethodInfo[] methodInfos = GetType().GetMethods();

            MethodInfo? startMethod = methodInfos.SingleOrDefault(method => method.GetCustomAttribute<ScriptStart>() != null);

            if (startMethod == null)
                throw new InvalidOperationException($"No start method for the script ({GetType().Name}) was found. Use the [ScriptStart] attribute to specify which method to use");

            ParameterInfo[] parameterInfos = startMethod.GetParameters();
            object?[] methodParameters = new object[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                object? parameterResult = null;

                ParameterInfo wantedParameter = parameterInfos[i];
                if (parameters != null && wantedParameter.Name != null)
                {
                    if (parameters.TryGetValue(wantedParameter.Name, out JsonNode? foundParameter))
                    {
                        if (foundParameter != null)
                        {
                            MethodInfo? getValueMethod = typeof(JsonNode).GetMethod("GetValue");

                            if (getValueMethod == null)
                                throw new Exception("The type JsonNode did not contain a method called GetValue when it was expected to");

                            MethodInfo getValueMethodWithRightType = getValueMethod.MakeGenericMethod(wantedParameter.ParameterType);
                            parameterResult = getValueMethodWithRightType.Invoke(foundParameter, null);
                        }
                    }
                }

                methodParameters[i] = parameterResult;
            }

            object? methodResult = startMethod.Invoke(this, methodParameters);

            return methodResult;
        }
    }
}
