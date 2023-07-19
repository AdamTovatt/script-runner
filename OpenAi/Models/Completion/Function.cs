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
    }
}
