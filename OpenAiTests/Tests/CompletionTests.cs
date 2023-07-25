using OpenAi;
using OpenAi.Helpers;
using OpenAi.Models.Completion;
using OpenAi.Models.Completion.Parameters;
using OpenAiTests.Utilities;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OpenAiTests.Tests
{
    [TestClass]
    public class CompletionTests
    {
        [TestMethod]
        public void DeserializeCompletionResult()
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
        public async Task IncludeFunctions()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            CompletionParameter completionParameter = new CompletionParameter(Model.Default);

            Function function = new Function("GetTheCurrentTime", "Will get the current time");
            function.Parameters.Add(new Parameter("timeZoneOffset", typeof(int), "The timezone of the user"), true);

            completionParameter.AddUserMessage("What is the current time?");
            completionParameter.AddFunction(function);

            CompletionResult completionResult = await openAi.CompleteAsync(completionParameter);

            Assert.IsNotNull(completionResult);
            Assert.IsNotNull(completionResult.Choices);
            Assert.AreEqual(1, completionResult.Choices.Count);

            Choice choice = completionResult.Choices[0];

            Assert.IsNotNull(choice);
            Assert.AreEqual("function_call", choice.FinishReason);
            Assert.IsNotNull(choice.Message.FunctionCall);
            Assert.AreEqual("GetTheCurrentTime", choice.Message.FunctionCall.Name);
            Assert.IsNotNull(choice.Message.FunctionCall.Arguments);
            Assert.AreEqual(1, choice.Message.FunctionCall.Arguments.Count);
            Assert.AreEqual(0, (int)choice.Message.FunctionCall.Arguments["timeZoneOffset"]);
        }

        [TestMethod]
        public async Task CompleteInAConversation()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            Conversation conversation = new Conversation(Model.Default);

            Function timeFunction = new Function("GetTimeInTimeZone", "Will get the time in the specified timezone");
            timeFunction.Parameters.Add(new Parameter("timeZoneOffset", typeof(int), "The timezone of the user"), true);
            conversation.Add(timeFunction);

            Function writeFunction = new Function("WriteStringToDatabase", "Will take a string input and write it to the database");
            writeFunction.Parameters.Add(new Parameter("valueToWrite", typeof(string), "The text to write to the database, has to be aquired from the user"), true);
            conversation.Add(writeFunction);

            CompletionParameter parameter = conversation.CreateCompletionParameter();
            parameter.AddSystemMessage("You have a number of functions at your disposal. If the user asks you for something that requires the use of a function you will make sure to get all the required parameters from the user and then call that function.");
            parameter.AddUserMessage("I would like write a text to the database");

            CompletionResult completionResult = await openAi.CompleteAsync(parameter);
            conversation.Add(completionResult);

            Assert.IsNotNull(completionResult);
            Assert.IsNotNull(completionResult.Choices);
            Assert.AreEqual(1, completionResult.Choices.Count);

            Choice choice = completionResult.Choices[0];
            Assert.IsNotNull(choice);
            Assert.AreEqual("stop", choice.FinishReason);

            parameter.AddUserMessage("Yes, of course. I would like to write the text \"Hello World\" to the database");
            completionResult = await openAi.CompleteAsync(parameter);
            conversation.Add(completionResult);

            Assert.IsNotNull(completionResult);
            Assert.IsNotNull(completionResult.Choices);

            choice = completionResult.Choices[0];
            Assert.IsNotNull(choice);
            Assert.AreEqual("function_call", choice.FinishReason);
        }

        [TestMethod]
        public void GenerateParameters()
        {
            string emptyParameterJson = new ParameterCollection().ToJson();
            Assert.AreEqual("{\"type\": \"object\", \"properties\": {}}", emptyParameterJson);

            ParameterCollection basicParameter = new ParameterCollection(); // test a single basic parameter that is not required
            basicParameter.Add(new Parameter("simpleString", typeof(string), "The string that the user should enter"));
            string basicParameterJson = basicParameter.ToJson();

            Assert.AreEqual("{\"type\": \"object\", \"properties\": {\"simpleString\": {\"type\": \"string\", \"description\": \"The string that the user should enter\"}}}", basicParameterJson);

            ParameterCollection parameterWithTwoValues = new ParameterCollection(); // test two parameters that are not required
            parameterWithTwoValues.Add(new Parameter("timeZoneOffset", typeof(int), "The timezone of the user"));
            parameterWithTwoValues.Add(new Parameter("name", typeof(string), "The name of the user"));
            string parameterWithTwoValuesJson = parameterWithTwoValues.ToJson();

            Assert.AreEqual("{\"type\": \"object\", \"properties\": {\"timeZoneOffset\": {\"type\": \"number\", \"description\": \"The timezone of the user\"}, \"name\": {\"type\": \"string\", \"description\": \"The name of the user\"}}}", parameterWithTwoValuesJson);

            ParameterCollection parameterWithTwoValues2 = new ParameterCollection(); // test two parameters that, where one of them is required
            parameterWithTwoValues2.Add(new Parameter("timeZoneOffset", typeof(int), "The timezone of the user"), true);
            parameterWithTwoValues2.Add(new Parameter("name", typeof(string), "The name of the user"));
            string parameterWithTwoValuesJson2 = parameterWithTwoValues2.ToJson();

            Assert.AreEqual("{\"type\": \"object\", \"properties\": {\"timeZoneOffset\": {\"type\": \"number\", \"description\": \"The timezone of the user\"}, \"name\": {\"type\": \"string\", \"description\": \"The name of the user\"}}, \"required\": [\"timeZoneOffset\"]}", parameterWithTwoValuesJson2);
        }

        [TestMethod]
        public void GetJsonTypeNames()
        {
            Assert.AreEqual("string", typeof(string).GetJsonTypeName());
            Assert.AreEqual("number", typeof(int).GetJsonTypeName());
            Assert.AreEqual("number", typeof(double).GetJsonTypeName());
            Assert.AreEqual("boolean", typeof(bool).GetJsonTypeName());
            Assert.AreEqual("object", typeof(object).GetJsonTypeName());
            Assert.AreEqual("array", typeof(List<int>).GetJsonTypeName());
            Assert.AreEqual("array", typeof(int[]).GetJsonTypeName());
        }

        [TestMethod]
        public void SerializeCompletionParamterWithFunction()
        {
            CompletionParameter completionParameter = new CompletionParameter(Model.Default);
            completionParameter.FunctionCall = "auto";

            Function function = new Function("GetTheCurrentTime", "Will get the current time");
            function.Parameters.Add(new Parameter("timeZoneOffset", typeof(int), "The offset"));

            completionParameter.AddFunction(function);

            string json = completionParameter.ToJson();

            Assert.AreEqual("{\r\n  \"model\": \"gpt-3.5-turbo\",\r\n  \"messages\": [],\r\n  \"functions\": [\r\n    {\r\n      \"name\": \"GetTheCurrentTime\",\r\n      \"description\": \"Will get the current time\",\r\n      \"parameters\": {\"type\": \"object\", \"properties\": {\"timeZoneOffset\": {\"type\": \"number\", \"description\": \"The offset\"}}}\r\n    }\r\n  ],\r\n  \"function_call\": \"auto\"\r\n}", json);
        }

        [TestMethod]
        public void SerializeMessageWithArguments()
        {
            Message message = new Message("assistant", "testContent");
            message.FunctionCall = new FunctionCall("GetTheCurrentTime", new Dictionary<string, JsonNode>());
            message.FunctionCall.Arguments!.Add("timeZoneOffset", 0);

            string json = JsonSerializer.Serialize(message);

            Assert.AreEqual("{\"role\":\"assistant\",\"content\":\"testContent\",\"name\":null,\"function_call\":{\"name\":\"GetTheCurrentTime\",\"arguments\":\"{\\\"timeZoneOffset\\\":0}\"}}", json);
        }
    }
}