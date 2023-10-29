using ScriptRunner.OpenAi.Models.Tuning;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Embeddings
{
    public class GetEmbeddingsResult : OpenAiApiResponse
    {
        [JsonPropertyName("data")]
        public EmbeddingData[] Data { get; set; }

        public GetEmbeddingsResult(EmbeddingData[] data)
        {
            Data = data;
        }

        public static GetEmbeddingsResult FromJson(string json)
        {
            GetEmbeddingsResult? result = JsonSerializer.Deserialize<GetEmbeddingsResult>(json);

            if (result == null) throw new JsonException($"Could not deserialize typeof @{typeof(GetEmbeddingsResult)} from json with lengt {json.Length}: {json}");
            return result;
        }
    }
}
