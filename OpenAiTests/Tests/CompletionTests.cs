using OpenAi;
using OpenAi.Models.Completion;
using OpenAiTests.Utilities;
using System.Text.Json;

namespace OpenAiTests.Tests
{
    [TestClass]
    public class CompletionTests
    {
        [TestMethod]
        public void CanDeserializeCompletionResult()
        {
            string json = "{\r\n  \"id\": \"chatcmpl-7e93tzyShui2zqD48gsCDGc6jjoPq\",\r\n  \"object\": \"chat.completion\",\r\n  \"created\": 1689802309,\r\n  \"model\": \"gpt-3.5-turbo-0613\",\r\n  \"choices\": [\r\n    {\r\n      \"index\": 0,\r\n      \"message\": {\r\n        \"role\": \"assistant\",\r\n        \"content\": \"joyful, glad, content\"\r\n      },\r\n      \"finish_reason\": \"stop\"\r\n    }\r\n  ],\r\n  \"usage\": {\r\n    \"prompt_tokens\": 31,\r\n    \"completion_tokens\": 6,\r\n    \"total_tokens\": 37\r\n  }\r\n}\r\n";
        
            CompletionResult? completionResult = JsonSerializer.Deserialize<CompletionResult>(json);

            Assert.IsNotNull(completionResult);
            Assert.IsNotNull(completionResult.Model);
            Assert.AreEqual(1, completionResult.Choices.Count);
            
            Choice choice = completionResult.Choices[0];

            Assert.IsNotNull(choice);
            Assert.IsNotNull(choice.FinishReason);
            Assert.AreEqual("stop", choice.FinishReason);
            Assert.IsNotNull(choice.Message);
            Assert.AreEqual("assistant", choice.Message.Role);
            Assert.AreEqual("joyful, glad, content", choice.Message.Content);

            Assert.AreEqual(completionResult.Created, 1689802309);
            Assert.AreEqual(completionResult.Id, "chatcmpl-7e93tzyShui2zqD48gsCDGc6jjoPq");
            Assert.AreEqual(completionResult.Model, "gpt-3.5-turbo-0613");
            Assert.AreEqual(completionResult.Object, "chat.completion");
        }

        [TestMethod]
        public async Task CompleteBasicMessage()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());
            
            CompletionParameter completionParameter = new CompletionParameter(Model.Default);
            completionParameter.AddSystemMessage("The user will provide you with a word and you will write three words that have the same meaning");
            completionParameter.AddUserMessage("happy");

            CompletionResult completionResult = await openAi.CompleteAsync(completionParameter);

            Assert.IsNotNull(completionResult);
            Assert.IsNotNull(completionResult.Model);
            Assert.AreEqual(1, completionResult.Choices.Count);

            Choice choice = completionResult.Choices[0];

            Assert.IsNotNull(choice);
            Assert.IsNotNull(choice.FinishReason);
            Assert.AreEqual("stop", choice.FinishReason);
            Assert.IsNotNull(choice.Message);
            Assert.AreEqual("assistant", choice.Message.Role);
            Assert.IsTrue(choice.Message.Content.Length > 10);

            Assert.IsTrue(completionResult.Created > 1689802309);
            Assert.IsTrue(!string.IsNullOrEmpty(completionResult.Id));
            Assert.IsTrue(!string.IsNullOrEmpty(completionResult.Model));
            Assert.AreEqual(completionResult.Object, "chat.completion");
        }

        [TestMethod]
        public async Task FunctionsCanBeIncluded()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            CompletionParameter completionParameter = new CompletionParameter(Model.Default);

            Function function = new Function("GetTheCurrentTime", "Will get the current time");
            function.AddParameter("timeZoneOffset", 1);

            completionParameter.AddUserMessage("What is the current time?");
            completionParameter.AddFunction(function);

            CompletionResult completionResult = await openAi.CompleteAsync(completionParameter);
        }
    }
}