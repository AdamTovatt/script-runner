using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Tuning
{
    public class ModelListResponse : OpenAiApiResponse
    {
        [JsonPropertyName("data")]
        public List<ModelInfo> Data { get; set; }

        public ModelListResponse()
        {
            Data = new List<ModelInfo>();
        }

        public static ModelListResponse FromJson(string json)
        {
            ModelListResponse? result = JsonSerializer.Deserialize<ModelListResponse>(json);

            if (result == null) throw new JsonException($"Could not deserialize typeof @{typeof(ModelListResponse)} from json with lengt {json.Length}: {json}");
            return result;
        }
    }
}
