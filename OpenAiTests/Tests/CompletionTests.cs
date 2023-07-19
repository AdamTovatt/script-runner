using OpenAi;
using OpenAi.Models.Completion;
using OpenAiTests.Utilities;

namespace OpenAiTests.Tests
{
    [TestClass]
    public class CompletionTests
    {
        [TestMethod]
        public async Task CompleteBasicMessage()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());
            
            CompletionParameter completionParameter = new CompletionParameter(Model.Default);
            completionParameter.AddSystemMessage("The user will provide you with a word and you will write three words that have the same meaning");
            completionParameter.AddUserMessage("happy");

            CompletionResult completionResult = await openAi.CompleteAsync(completionParameter);

            Assert.IsNotNull(completionResult);
            Assert.IsNotNull(completionResult.Choices);
            Assert.IsTrue(completionResult.Choices.Count > 0);
        }
    }
}