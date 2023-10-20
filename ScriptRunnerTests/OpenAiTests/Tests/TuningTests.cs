using OpenAiTests.Utilities;
using ScriptRunner.OpenAi;
using ScriptRunner.OpenAi.Models.Files;
using ScriptRunner.OpenAi.Models.Tuning;

namespace ScriptRunnerTests.OpenAiTests.Tests
{
    [TestClass]
    public class TuningTests
    {
        private static string? trainingFileId;

        [ClassInitialize]
        public static async Task BeforeAll(TestContext testContext)
        {
            const string fileContent = "{\"messages\":[{\"role\":\"system\",\"content\":\"You are an assistant that occasionally misspells words\"},{\"role\":\"user\",\"content\":\"Tell me a story.\"},{\"role\":\"assistant\",\"content\":\"One day a student went to schoool.\"}]}\r\n{\"messages\":[{\"role\":\"system\",\"content\":\"You are an assistant that occasionally misspells words\"},{\"role\":\"user\",\"content\":\"Tell me a story.\"},{\"role\":\"assistant\",\"content\":\"One day a student went to schoool.\"}]}";

            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            UploadFileResult uploadFileResult = await openAi.UploadFileAsync(fileContent);
            trainingFileId = uploadFileResult.Id;
        }

        [ClassCleanup]
        public static async Task AfterAll()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            if (trainingFileId != null)
            {
                DeleteFileResult deleteFileResult = await openAi.DeleteFileAsync(trainingFileId);

                if (!deleteFileResult.Deleted)
                    throw new Exception("There was an error when cleaning up tests, the file could not be deleted");
            }
        }

        [TestMethod]
        public async Task StartGetListAndCancelTuningJob()
        {
            Assert.IsNotNull(trainingFileId, "There has probably been some error with uploading the training file that will be used for the tests");

            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            TuningJobResponse startTuningJobResponse = await openAi.StartTuningJobAsync(trainingFileId, "gpt-3.5-turbo");

            Assert.IsNull(startTuningJobResponse.Error);
            Assert.IsNotNull(startTuningJobResponse.Id);
            Assert.IsNotNull(startTuningJobResponse.Model);
            Assert.IsNotNull(startTuningJobResponse.TrainingFile);

            TuningJobResponse getTuningJobResponse = await openAi.GetTuningJobAsync(startTuningJobResponse.Id);

            Assert.IsNull(getTuningJobResponse.Error);
            Assert.IsNotNull(getTuningJobResponse.Id);
            Assert.IsNotNull(getTuningJobResponse.Model);

            TuningJobEventResponse tuningJobEventResponse = await openAi.GetTuningJobEventsAsync(startTuningJobResponse.Id);

            Assert.IsNull(tuningJobEventResponse.Error);
            Assert.AreNotEqual(0, tuningJobEventResponse.Data.Count);

            TuningJobEvent firstEvent = tuningJobEventResponse.Data[0];

            Assert.IsNotNull(firstEvent);
            Assert.IsNotNull(firstEvent.Message);

            TuningJobResponse cancelResponse = await openAi.CancelTuningJobAsync(startTuningJobResponse.Id);

            Assert.IsNull(cancelResponse.Error);
            Assert.AreEqual("cancelled", cancelResponse.Status);
        }

        [TestMethod]
        public async Task UploadAndDeleteFile()
        {
            const string fileContent = "{\"messages\":[{\"role\":\"system\",\"content\":\"You are an assistant that occasionally misspells words\"},{\"role\":\"user\",\"content\":\"Tell me a story.\"},{\"role\":\"assistant\",\"content\":\"One day a student went to schoool.\"}]}\r\n{\"messages\":[{\"role\":\"system\",\"content\":\"You are an assistant that occasionally misspells words\"},{\"role\":\"user\",\"content\":\"Tell me a story.\"},{\"role\":\"assistant\",\"content\":\"One day a student went to schoool.\"}]}";

            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            UploadFileResult uploadFileResult = await openAi.UploadFileAsync(fileContent);

            Assert.IsNotNull(uploadFileResult);
            Assert.IsNull(uploadFileResult.Error);
            Assert.IsNotNull(uploadFileResult.Id);

            await Task.Delay(10000); // otherwise openai gives us an error because "the file is still processing"

            DeleteFileResult deleteFileResult = await openAi.DeleteFileAsync(uploadFileResult.Id);

            Assert.IsNotNull(deleteFileResult);
            Assert.IsTrue(deleteFileResult.Deleted);
            Assert.IsNull(deleteFileResult.Error);
        }

        [TestMethod]
        public async Task InvalidUpdateAndDeleteReturnsError()
        {
            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            UploadFileResult uploadFileResult = await openAi.UploadFileAsync("Invalid file data :)");

            Assert.IsNotNull(uploadFileResult);
            Assert.IsNotNull(uploadFileResult.Error);
            Assert.IsNotNull(uploadFileResult.Error.Message);

            DeleteFileResult deleteFileResult = await openAi.DeleteFileAsync("obviouslyNotARealId");

            Assert.IsNotNull(deleteFileResult);
            Assert.IsNotNull(deleteFileResult.Error);
            Assert.IsFalse(deleteFileResult.Deleted);
            Assert.IsNotNull(deleteFileResult.Error.Message);
        }
    }
}
