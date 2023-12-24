using OpenAiTests.Utilities;
using ScriptRunner.OpenAi;
using ScriptRunner.OpenAi.Models.Tuning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptRunnerTests.OpenAiTests.Tests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public async Task GetModels()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            ModelListResponse modelListResponse = await openAi.GetModelsAsync();

            Assert.IsNotNull(modelListResponse);
            Assert.IsNull(modelListResponse.Error);
            Assert.IsNotNull(modelListResponse.Data);
            Assert.AreNotEqual(0, modelListResponse.Data.Count);

            ModelInfo model = modelListResponse.Data[0];

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.Id);
            Assert.IsNotNull(model.Permissions);
        }
    }
}
