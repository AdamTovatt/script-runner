using OpenAi.Converters;
using OpenAi.Models.Completion.Parameters;
using System.Text.Json.Serialization;

namespace OpenAi.Models.Completion
{
    public class Function
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonConverter(typeof(ParameterCollectionConverter))]
        public ParameterCollection Parameters { get; set; }

        public Function(string name, string description, ParameterCollection parameters)
        {
            Name = name;
            Description = description;
            Parameters = parameters;
        }

        public Function(string name, string description)
        {
            Name = name;
            Description = description;
            Parameters = new ParameterCollection();
        }

        public void AddParameter(string name, Type type, string description, bool required)
        {
            Parameters.Add(new Parameter(name, type, description), required);
        }
    }
}
