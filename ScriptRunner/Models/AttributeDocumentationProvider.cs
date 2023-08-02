using ScriptRunner.DocumentationAttributes;
using System.Reflection;

namespace ScriptRunner.Models
{
    public class AttributeDocumentationProvider : IDocumentationProvider
    {
        public AttributeDocumentationProvider(MethodInfo methodInfo)
        {
            Parameters = new Dictionary<string, string>();

            IEnumerable<Parameter> parameters = methodInfo.GetCustomAttributes<Parameter>();

            foreach (Parameter parameter in parameters)
                Parameters.Add(parameter.Name, parameter.Description);

            Summary? summary = methodInfo.GetCustomAttribute<Summary>();

            if (summary != null)
                Summary = summary.Text;

            Returns? returns = methodInfo.GetCustomAttribute<Returns>();

            if (returns != null)
                Returns = returns.Text;

            AllowedRoles? allowedRoles = methodInfo.GetCustomAttribute<AllowedRoles>();

            if(allowedRoles != null)
                AllowedRoles = allowedRoles.Roles;
        }

        public string? Summary { get; set; }
        public Dictionary<string, string>? Parameters { get; set; }
        public string? Returns { get; set; }
        public string[]? AllowedRoles { get; set; }

        public string GetParameterDescription(string parameterName)
        {
            if (Parameters == null)
                return "";

            if (Parameters.TryGetValue(parameterName, out string? description))
                return description;

            return "";
        }
    }
}
