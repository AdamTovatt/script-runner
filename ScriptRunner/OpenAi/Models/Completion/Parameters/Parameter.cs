using ScriptRunner.OpenAi.Helpers;

namespace OpenAi.Models.Completion.Parameters
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        private string? genericType;

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
