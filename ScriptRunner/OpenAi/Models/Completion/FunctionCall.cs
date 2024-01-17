using ScriptRunner.OpenAi.Converters;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Completion
{
    /// <summary>
    /// Represents a function call that OpenAi wants to do
    /// </summary>
    public class FunctionCall
    {
        /// <summary>
        /// The name of the function that OpenAi wants to call
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The arguments to pass to the function, as a json string that can be deserialized into an object
        /// </summary>
        [JsonPropertyName("arguments")]
        [JsonConverter(typeof(ArgumentsConverter))]
        public Dictionary<string, JsonNode>? Arguments { get; set; }

        public FunctionCall(string name, Dictionary<string, JsonNode>? arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        /// <summary>
        /// Will create a function call from a given name of a function
        /// </summary>
        /// <param name="name">The name of the function</param>
        /// <returns>A function call</returns>
        public static FunctionCall FromName(string name)
        {
            return new FunctionCall(name, null);
        }

        /// <summary>
        /// Will deserialize a function call from a json string
        /// </summary>
        /// <param name="json">The json to deserialize</param>
        /// <returns>A function call</returns>
        /// <exception cref="Exception"></exception>
        public static FunctionCall FromJson(string json)
        {
            FunctionCall? result = JsonSerializer.Deserialize<FunctionCall>(json);

            if (result == null)
                throw new Exception("Could not deserialize function call");

            return result;
        }

        /// <summary>
        /// Will create a function call from a compiled script. Specify the compiled script type as the generic parameter. If you want to you can add arguments, but they are optional.
        /// </summary>
        /// <typeparam name="T">The type of the compiled script to call</typeparam>
        /// <param name="arguments">Optional arguments dictionary</param>
        /// <returns>A function call</returns>
        public static FunctionCall FromCompiledScript<T>(Dictionary<string, JsonNode>? arguments = null) where T : CompiledScript
        {
            MethodInfo? startMethod = typeof(T).GetMethods().SingleOrDefault(method => method.GetCustomAttribute<ScriptStart>() != null);

            if (startMethod == null)
                throw new InvalidOperationException($"Tried to create a function call to a script ({typeof(T).Name}) but it was missing the [ScriptStart] attribute to specify which method to use");

            return new FunctionCall(startMethod.Name, arguments);
        }
    }
}
