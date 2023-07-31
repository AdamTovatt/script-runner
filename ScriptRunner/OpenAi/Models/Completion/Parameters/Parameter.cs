using ScriptRunner.OpenAi.Helpers;

namespace OpenAi.Models.Completion.Parameters
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public Parameter(string name, Type type, string description)
        {
            Name = name;
            Description = description;
            Type = type.GetJsonTypeName();
        }

        public override string ToString()
        {
            return $"{Name}({Type}), {Description}";
        }

        public string ToJson()
        {
            return $"\"{Name}\": {{\"type\": \"{Type}\", \"description\": \"{Description}\"}}";
        }
    }
}
