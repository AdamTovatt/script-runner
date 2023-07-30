using System.Text.Json.Serialization;

namespace SkippyBackend.FrontEndModels
{
    public class DisplayMessage
    {
        [JsonPropertyName("align")]
        public int Align { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        public DisplayMessage(string text, string color, int align)
        {
            Text = text;
            Color = color;
            Align = align;
        }
    }
}
