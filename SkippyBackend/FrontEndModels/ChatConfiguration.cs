namespace SkippyBackend.FrontEndModels
{
    public class ChatConfiguration
    {
        public Dictionary<string, string> Colors { get; set; } = new Dictionary<string, string>
        {
            { "Depth0", "#242728" },
            { "Depth1", "#2E3235" },
            { "Depth2", "#363B3E" },
            { "Depth3", "#3F4447" },
            { "Depth4", "#474C4F" },
            { "Depth5", "#4F5356" },
            { "Depth6", "#585D60" },
            { "Depth7", "#606568" },
            { "Depth8", "#686D71" },
            { "Depth9", "#494D50" },
            { "Accent1", "#007BFF" },
            { "Accent2", "#999999" },
            { "Success", "#28A745" },
            { "Error", "#DC3545" },
            { "Warning", "#FFC107" },
            { "White", "#FFFFFF" },
        };
    }
}