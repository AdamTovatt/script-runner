using System.Text.Json.Nodes;

namespace OpenAi.Models.Completion
{
    public class Function
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public JsonObject Parameters { get; set; }

        public Function(string name, string description, JsonObject parameters)
        {
            Name = name;
            Description = description;
            Parameters = parameters;
        }

        public Function(string name, string description)
        {
            Name = name;
            Description = description;
            Parameters = new JsonObject();
        }

        public void AddParameter(string propertyName, object value)
        {
            Parameters.Add(propertyName, JsonValue.Create(value));
        }
    }
}
