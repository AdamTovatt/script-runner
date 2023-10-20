using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Tuning
{
    public class TuningJobEvent
    {
        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
        [JsonPropertyName("level")]
        public string Level { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }

        public TuningJobEvent(long createdAt, string level, string message, string type)
        {
            CreatedAt = createdAt;
            Level = level;
            Message = message;
            Type = type;
        }
    }
}
