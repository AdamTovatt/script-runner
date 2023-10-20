using ScriptRunner.OpenAi.Models;
using ScriptRunner.OpenAi.Models.Files;

namespace ScriptRunnerTests.OpenAiTests.Tests
{
    [TestClass]
    public class DeserializeTests
    {
        [TestMethod]
        public void DeserializeErrorReponse()
        {
            const string json = "{\r\n    \"error\": {\r\n        \"message\": \"Uploaded empty file. Please upload a file with data.\",\r\n        \"type\": \"invalid_request_error\",\r\n        \"param\": null,\r\n        \"code\": null\r\n    }\r\n}";

            UploadFileResult uploadFileResult = UploadFileResult.FromJson(json);

            Assert.IsNotNull(uploadFileResult.Error);
            Assert.IsNotNull(uploadFileResult.Error.Type);
            Assert.IsNotNull(uploadFileResult.Error.Message);
        }

        [TestMethod]
        public void DeserializeUploadFileResponse()
        {
            const string json = "{\r\n    \"object\": \"file\",\r\n    \"id\": \"file-DjKCYftsT1TyJBVIL9VAIsbT\",\r\n    \"purpose\": \"fine-tune\",\r\n    \"filename\": \"testTrainingData.txt\",\r\n    \"bytes\": 2288,\r\n    \"created_at\": 1697802791,\r\n    \"status\": \"uploaded\",\r\n    \"status_details\": null\r\n}";

            UploadFileResult result = UploadFileResult.FromJson(json);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Object);
            Assert.IsNotNull(result.Id);
            Assert.IsNotNull(result.Purpose);
        }
    }
}
