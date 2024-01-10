using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace ScriptRunner
{
    /// <summary>
    /// Represents a compiled script, is also the class that created scripts should inherit from
    /// </summary>
    public class CompiledScript
    {
        /// <summary>
        /// The context that the script runs in. Can contain usefull information for the script
        /// </summary>
        public ScriptContext Context { get; set; }

        /// <summary>
        /// Constructor taking a script context as a parameter
        /// </summary>
        /// <param name="context">The context that the script runs in</param>
        public CompiledScript(ScriptContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Will run a script with the specified parameters
        /// </summary>
        /// <param name="parameters">The parameters to use</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If the script is missing a method with the [ScriptStart] attribute</exception>
        /// <exception cref="MissingMethodException">Will be thrown if the JsonNode type is missing the GetValue method, this should really never happen unless the actual System.Text.Json.Nodes library is changed</exception>
        public object? Run(Dictionary<string, JsonNode>? parameters)
        {
            MethodInfo[] methodInfos = GetType().GetMethods();

            MethodInfo? startMethod = methodInfos.SingleOrDefault(method => method.GetCustomAttribute<ScriptStart>() != null);

            if (startMethod == null)
                throw new InvalidOperationException($"No start method for the script ({GetType().Name}) was found. Use the [ScriptStart] attribute to specify which method to use");

            ParameterInfo[] parameterInfos = startMethod.GetParameters();
            object?[] methodParameters = new object[parameterInfos.Length];

            MethodInfo? getValueMethod = typeof(JsonNode).GetMethod("GetValue");

            if (getValueMethod == null)
                throw new MissingMethodException("The type JsonNode did not contain a method called GetValue when it was expected to");

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
                            if (!typeof(IEnumerable<object>).IsAssignableFrom(wantedParameter.ParameterType)) // just a normal type
                            {
                                MethodInfo getValueMethodWithRightType = getValueMethod.MakeGenericMethod(wantedParameter.ParameterType);
                                parameterResult = getValueMethodWithRightType.Invoke(foundParameter, null);
                            }
                            else // some sort of collection
                            {
                                Type? genericType = wantedParameter.ParameterType.IsArray ? wantedParameter.ParameterType.GetElementType() : wantedParameter.ParameterType.GenericTypeArguments[0];
                                if (genericType == null) throw new Exception("Missing generic type for collection");

                                Type listType = typeof(List<>).MakeGenericType(genericType);
                                object? list = Activator.CreateInstance(listType);
                                MethodInfo? addMethod = list?.GetType().GetMethod("Add");
                                if (addMethod == null) throw new Exception("Missing add method for list");

                                foreach (JsonNode? node in foundParameter.AsArray())
                                {
                                    MethodInfo getValueMethodWithRightType = getValueMethod.MakeGenericMethod(genericType);
                                    addMethod.Invoke(list, new object?[] { getValueMethodWithRightType.Invoke(node, null) });
                                }

                                if (wantedParameter.ParameterType.IsArray)
                                {
                                    MethodInfo? toArrayMethod = list?.GetType().GetMethod("ToArray");
                                    if (toArrayMethod == null) throw new Exception("Missing ToArray method for list");
                                    parameterResult = toArrayMethod.Invoke(list, null);
                                }
                                else
                                {
                                    parameterResult = list;
                                }
                            }
                        }
                    }
                }

                methodParameters[i] = parameterResult;
            }

            object? methodResult = null;
            if (GetIsAsyncMethod(startMethod))
            {
                Task.Run(async () =>
                {
                    dynamic? task = startMethod.Invoke(this, methodParameters);
                    if (task != null)
                        methodResult = await task;
                }).Wait();
            }
            else
            {
                methodResult = startMethod.Invoke(this, methodParameters);
            }

            return methodResult;
        }

        private bool GetIsAsyncMethod(MethodInfo method)
        {
            return method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
        }
    }
}
