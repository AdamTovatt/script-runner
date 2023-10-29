using OpenAiTests.Utilities;
using ScriptRunner.OpenAi;
using ScriptRunner.OpenAi.Models.Embeddings;

namespace ScriptRunnerTests.OpenAiTests.Tests
{
    [TestClass]
    public class EmbeddingsTests
    {
        [TestMethod]
        public async Task GetEmbeddings()
        {
            const string input = "This is a test string to get embeddings for";

            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            GetEmbeddingsResult getResult = await openAi.GetEmbeddingsAsync(input);

            Assert.IsNotNull(getResult);
            Assert.IsNull(getResult.Error);
            Assert.IsNotNull(getResult.Data);
            Assert.AreEqual(1, getResult.Data.Length);

            EmbeddingData embeddingData = getResult.Data[0];

            Assert.AreEqual(0, embeddingData.Index);
            Assert.IsNotNull(embeddingData.Embedding);
            Assert.AreEqual(1536, embeddingData.Embedding.Length);
        }
    }
}
