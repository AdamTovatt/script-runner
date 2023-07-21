using System.Text;

namespace OpenAi.Models.Completion.Parameters
{
    public class ParameterCollection
    {
        public string Type { get; set; }
        public List<Parameter>? Parameters { get; set; }
        public List<Parameter>? RequiredParameters { get; set; }

        public ParameterCollection()
        {
            Type = "object";
        }

        public void AddParameter(Parameter parameter)
        {
            AddParameter(parameter, false);
        }

        public void AddParameter(Parameter parameter, bool required)
        {
            if (Parameters == null)
                Parameters = new List<Parameter>();

            Parameters.Add(parameter);

            if (required)
                AddRequiredParameter(parameter);
        }

        public void AddRequiredParameter(Parameter parameter)
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
                foreach(Parameter parameter in RequiredParameters)
                {
                    if(shouldAddComma)
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
