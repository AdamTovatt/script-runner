using OpenAiTests.Utilities;
using ScriptRunner.OpenAi;
using ScriptRunner.OpenAi.Models.Files;

namespace ScriptRunnerTests.OpenAiTests.Tests
{
    [TestClass]
    public class FileTests
    {
        [TestMethod]
        public async Task UploadAndDeleteFile()
        {
            const string fileContent = "{\"messages\":[{\"role\":\"system\",\"content\":\"You are an assistant that occasionally misspells words\"},{\"role\":\"user\",\"content\":\"Tell me a story.\"},{\"role\":\"assistant\",\"content\":\"One day a student went to schoool.\"}]}\r\n{\"messages\":[{\"role\":\"system\",\"content\":\"You are an assistant that occasionally misspells words\"},{\"role\":\"user\",\"content\":\"Tell me a story.\"},{\"role\":\"assistant\",\"content\":\"One day a student went to schoool.\"}]}";

            OpenAiApi openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());

            UploadFileResult uploadFileResult = await openAi.UploadFileAsync(fileContent);

            Assert.IsNotNull(uploadFileResult);
            Assert.IsNull(uploadFileResult.Error);
            Assert.IsNotNull(uploadFileResult.Id);

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
