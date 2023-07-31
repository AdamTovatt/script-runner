using System.Xml.Linq;

namespace ScriptRunner.Models
{
    public class XmlComment : ICommentProvider
    {
        public string? Summary { get; set; }
        public Dictionary<string, string>? Parameters { get; set; }
        public string? Returns { get; set; }

        public XmlComment(string rawComment)
        {
            if (string.IsNullOrWhiteSpace(rawComment))
                return;

            XDocument xDocument = XDocument.Parse("<root>" + rawComment + "</root>");
            XElement? summaryElement = xDocument.Descendants("summary").FirstOrDefault();
            XElement? returnsElement = xDocument.Descendants("returns").FirstOrDefault();

            Summary = GetElementValueWithoutSlashes(summaryElement);
            Returns = GetElementValueWithoutSlashes(returnsElement);

            foreach (XElement? decendant in xDocument.Descendants("param"))
            {
                if (decendant == null) continue;

                string? key = decendant.Attribute("name")?.Value;
                string value = decendant.Value.Trim();

                if (key == null || value == null) continue;

                if (Parameters == null)
                    Parameters = new Dictionary<string, string>();

                Parameters.Add(key, value);
            }
        }

        private string? GetElementValueWithoutSlashes(XElement? element)
        {
            if (element == null)
                return null;

            return element?.Value.Trim().Replace("///", string.Empty).Trim();
        }

        public string GetParameterDescription(string parameterName)
        {
            if (Parameters == null)
                return string.Empty;

            if (Parameters.ContainsKey(parameterName))
                return Parameters[parameterName];

            return string.Empty;
        }
    }
}
