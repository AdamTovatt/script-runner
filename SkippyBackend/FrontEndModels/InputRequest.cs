using ScriptRunner.OpenAi.Models.Input;
using System.Text.Json.Serialization;

namespace SkippyBackend.FrontEndModels
{
    public class InputRequest
    {
        [JsonPropertyName("info")]
        public InputInfo Info { get; set; }

        [JsonPropertyName("displayMessage")]
        public DisplayMessage DisplayMessage { get; set; }

        public InputRequest(InputInfo info, DisplayMessage displayMessage)
        {
            Info = info;
            DisplayMessage = displayMessage;
        }
    }
}
