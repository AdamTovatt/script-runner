using System.Text.Json.Serialization;

namespace SkippyBackend.FrontEndModels
{
    public class InfoMessage
    {
        public enum InfoMessageType { Success, Error, Warning, Info }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("type")]
        public string Type => MessageType.ToString().ToLower();

        [JsonIgnore]
        public InfoMessageType MessageType { get; set; }

        public InfoMessage(string text, InfoMessageType type)
        {
            Text = text;
            MessageType = type;
        }
    }
}
