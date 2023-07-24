namespace Console
{
    public static class EnvironmentHelper
    {
        public const string OpenAiKeyPath = "openai.txt";

        public static string GetOpenAiApiKey()
        {
            if (!File.Exists(OpenAiKeyPath))
                throw new FileNotFoundException($"OpenAI key file not found. Should be a file called {OpenAiKeyPath} with an api key.");

            return File.ReadAllText(OpenAiKeyPath);
        }
    }
}
