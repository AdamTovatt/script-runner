using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Tuning
{
    public class TuningJobEventResponse : OpenAiApiResponse
    {
        [JsonPropertyName("data")]
        public List<TuningJobEvent> Data { get; set; }
        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }

        public TuningJobEventResponse(bool hasMore)
        {
            HasMore = hasMore;
            Data = new List<TuningJobEvent>();
        }

        public static TuningJobEventResponse FromJson(string json)
        {
            TuningJobEventResponse? result = JsonSerializer.Deserialize<TuningJobEventResponse>(json);

            if (result == null) throw new JsonException($"Could not deserialize typeof @{typeof(TuningJobEventResponse)} from json with lengt {json.Length}: {json}");
            return result;
        }
    }
}
