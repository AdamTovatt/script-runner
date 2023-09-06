using ScriptRunner.Models;
using System.Text.Json.Serialization;

namespace SkippyBackend.FrontEndModels
{
    public class DisplayMessage
    {
        [JsonPropertyName("align")]
        public int Align { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("contentType")]
        public ContentType ContentType { get; set; }

        [JsonPropertyName("fileName")]
        public string? FileName { get; set; }

        public DisplayMessage(string text, string color, int align)
        {
            Content = text;
            Color = color;
            Align = align;
            ContentType = ContentType.Text;
        }

        public DisplayMessage(string content, string color, int align, ContentType contentType, string? fileName)
        {
            Content = content;
            Color = color;
            ContentType = contentType;
            Align = align;
            FileName = fileName;
        }
    }
}
