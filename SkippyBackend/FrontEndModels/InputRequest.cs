using System.Text.Json.Serialization;

namespace SkippyBackend.FrontEndModels
{
    public class InputRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        public InputRequest(Type type, string description)
        {
            Type = type.ToString();
            Description = description;
        }
    }
}
