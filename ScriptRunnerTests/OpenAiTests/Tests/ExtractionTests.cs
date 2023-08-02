using OpenAiTests.Utilities;
using ScriptRunner.OpenAi;
using ScriptRunner.OpenAi.Models.Completion;
using ScriptRunner.OpenAi.Models.InputTypes;

namespace ScriptRunnerTests.OpenAiTests.Tests
{
    [TestClass]
    public class ExtractionTests
    {
        [TestMethod]
        public async Task ExtractInteger()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            ExtractionResult<IntInputType> extractedValue = await openAi.ExtractAsync<IntInputType>("It's 500£");

            Assert.IsTrue(extractedValue.Valid);
            Assert.AreEqual(500, extractedValue.ExtractedValue!.Value);
        }

        [TestMethod]
        public async Task ExtractDecimal()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            ExtractionResult<DecimalInputType> extractedValue = await openAi.ExtractAsync<DecimalInputType>("A value is 70.2");

            Assert.IsTrue(extractedValue.Valid);
            Assert.AreEqual(70.2m, extractedValue.ExtractedValue!.Value);
        }

        [TestMethod]
        public async Task ExtractDecimal2()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            ExtractionResult<DecimalInputType> extractedValue = await openAi.ExtractAsync<DecimalInputType>("around 400£ I think");

            Assert.IsTrue(extractedValue.Valid);
            Assert.AreEqual(400m, extractedValue.ExtractedValue!.Value);
        }

        [TestMethod]
        public async Task ExtractBoolTrue()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            ExtractionResult<BoolInputType> extractedValue = await openAi.ExtractAsync<BoolInputType>("Yes, I think so");

            Assert.IsTrue(extractedValue.Valid);
            Assert.IsTrue(extractedValue.ExtractedValue!.Value);
        }

        [TestMethod]
        public async Task ExtractBoolFalse()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            ExtractionResult<BoolInputType> extractedValue = await openAi.ExtractAsync<BoolInputType>("Not really");

            Assert.IsTrue(extractedValue.Valid);
            Assert.IsFalse(extractedValue.ExtractedValue!.Value);
        }

        [TestMethod]
        public async Task ExtractSubject()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            ExtractionResult<StringInputType> extractedValue = await openAi.ExtractAsync<StringInputType>("I'd like to insure my bike");

            Assert.IsTrue(extractedValue.Valid);
            Assert.AreEqual("bike", extractedValue.ExtractedValue!.Value);
        }
    }
}
