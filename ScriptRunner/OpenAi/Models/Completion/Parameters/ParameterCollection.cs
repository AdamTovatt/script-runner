using OpenAi.Models.Completion.Parameters;
using System.Text;
using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Completion.Parameters
{
    public class ParameterCollection
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("properties")]
        public List<Parameter>? Parameters { get; set; }

        [JsonPropertyName("required")]
        public List<Parameter>? RequiredParameters { get; set; }

        public ParameterCollection()
        {
            Type = "object";
        }

        public void Add(Parameter parameter)
        {
            Add(parameter, false);
        }

        public void Add(Parameter parameter, bool required)
        {
            if (Parameters == null)
                Parameters = new List<Parameter>();

            Parameters.Add(parameter);

            if (required)
                AddRequired(parameter);
        }

        public void AddRequired(Parameter parameter)
        {
            if (RequiredParameters == null)
                RequiredParameters = new List<Parameter>();

            RequiredParameters.Add(parameter);
        }

        public string ToJson()
        {
            StringBuilder jsonBuilder = new StringBuilder();

            jsonBuilder.Append("{\"type\": \"");
            jsonBuilder.Append(Type);
            jsonBuilder.Append("\", \"properties\": {");

            if (Parameters != null) // add the normal paramteres
            {
                bool shouldAddComma = false;
                foreach (Parameter parameter in Parameters)
                {
                    if (shouldAddComma)
                        jsonBuilder.Append(", ");

                    jsonBuilder.Append(parameter.ToJson());
                    shouldAddComma = true;
                }
            }

            jsonBuilder.Append("}");

            if (RequiredParameters != null) // add the required parameters list
            {
                jsonBuilder.Append(", \"required\": [");
                bool shouldAddComma = false;
                foreach (Parameter parameter in RequiredParameters)
                {
                    if (shouldAddComma)
                        jsonBuilder.Append(", ");

                    jsonBuilder.Append($"\"{parameter.Name}\"");
                    shouldAddComma = true;
                }
                jsonBuilder.Append("]");
            }

            jsonBuilder.Append("}");

            return jsonBuilder.ToString();
        }
    }
}
