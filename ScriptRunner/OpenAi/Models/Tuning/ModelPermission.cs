using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Tuning
{
    public class ModelPermission
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("allow_create_engine")]
        public bool AllowCreateEngine { get; set; }
        [JsonPropertyName("allow_fine_tuning")]
        public bool AllowFineTuning { get; set; }

        public ModelPermission(string id, bool allowCreateEngine, bool allowFineTuning)
        {
            Id = id;
            AllowCreateEngine = allowCreateEngine;
            AllowFineTuning = allowFineTuning;
        }
    }
}
