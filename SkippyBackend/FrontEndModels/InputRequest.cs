using System.Text.Json.Serialization;

namespace SkippyBackend.FrontEndModels
{
    public class InputRequest
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("displayMessage")]
        public DisplayMessage DisplayMessage { get; set; }

        public InputRequest(Type type, DisplayMessage displayMessage)
        {
            Type = type.ToString();
            DisplayMessage = displayMessage;
        }
    }
}
