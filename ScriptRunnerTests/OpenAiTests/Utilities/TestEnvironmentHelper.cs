namespace OpenAiTests.Utilities
{
    public static class TestEnvironmentHelper
    {
        public const string OpenAiKeyPath = "openai.txt";

        public static string GetOpenAiApiKey()
        {
            return File.ReadAllText(OpenAiKeyPath);
        }
    }
}
