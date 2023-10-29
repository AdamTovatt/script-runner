using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Embeddings
{
    public class EmbeddingData
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }
        [JsonPropertyName("embedding")]
        public float[] Embedding { get; set; }

        public EmbeddingData(int index, float[] embedding)
        {
            Index = index;
            Embedding = embedding;
        }
    }
}
