using OpenAiTests.Utilities;

namespace OpenAiTests.Tests
{
    [TestClass]
    public class EnvironmentTests
    {
        [TestMethod]
        public void OpenAiKeyFileExists()
        {
            Assert.IsTrue(File.Exists(TestEnvironmentHelper.OpenAiKeyPath), $"The {TestEnvironmentHelper.OpenAiKeyPath} file could not be found");
        }

        [TestMethod]
        public void OpenAiKeyFileHasContent()
        {
            Assert.IsTrue(!string.IsNullOrEmpty(File.ReadAllText(TestEnvironmentHelper.OpenAiKeyPath)), $"The {TestEnvironmentHelper.OpenAiKeyPath} file is missing content");
        }

        [TestMethod]
        public void OpenAiKeyCanBeLoaded()
        {
            Assert.IsTrue(!string.IsNullOrEmpty(TestEnvironmentHelper.GetOpenAiApiKey()));
        }
    }
}
