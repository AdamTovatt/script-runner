using ScriptRunner.OpenAi.Helpers;
using ScriptRunner.OpenAi.Models.Completion;

namespace OpenAi.Models.Completion.Parameters
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        private string? genericType;

        /// <summary>
        /// The default constructor for parameter that should be used
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="type">The type of the parameter as a c# type</param>
        /// <param name="description">The description of the parameter</param>
        public Parameter(string name, Type type, string description)
        {
            Name = name;
            Description = description;
            Type = type.GetJsonTypeName();

            if (type.IsGenericType)
                genericType = type.GetGenericArguments()[0].GetJsonTypeName();
            if (type.IsArray)
                genericType = type.GetElementType()?.GetJsonTypeName();
        }

        /// <summary>
        /// Constructor for manually setting the values (prefer to use the other constructor) if you need to use this one
        /// maybe because you don't have c# types
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        /// <param name="description">The description of the parameter</param>
        /// <param name="type">The type of the parameter (as a json type)</param>
        /// <param name="genericType"></param>
        public Parameter(string name, string description, string type, string? genericType)
        {
            if (!JsonType.IsValid(type))
                throw new ArgumentException($"The given type \"{type}\" is not a valid json type. Should be one of {JsonType.GetPossibleValuesAsString()}");

            if (genericType != null && !JsonType.IsValid(type))
                throw new ArgumentException($"The given type \"{genericType}\" is not a valid json type. Should be one of {JsonType.GetPossibleValuesAsString()}");

            Name = name;
            Description = description;
            Type = type;
            this.genericType = genericType;
        }

        public override string ToString()
        {
            return $"{Name}({Type}), {Description}";
        }

        public string ToJson()
        {
            if (Type == "array")
                return $"\"{Name}\": {{\"type\": \"{Type}\", \"description\": \"{Description}\", \"items\": {{\"type\": \"{genericType}\"}}}}";
            return $"\"{Name}\": {{\"type\": \"{Type}\", \"description\": \"{Description}\"}}";
        }
    }
}
