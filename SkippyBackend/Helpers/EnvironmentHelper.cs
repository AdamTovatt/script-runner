namespace SkippyBackend.Helpers
{
    public class EnvironmentHelper
    {
        private static Dictionary<string, string> variableCache = new Dictionary<string, string>();

        public static string GetEnvironmentVariable(string environmentVariableName, int minLength = 0)
        {
            if (variableCache.ContainsKey(environmentVariableName))
                return variableCache[environmentVariableName];

            string? result = Environment.GetEnvironmentVariable(environmentVariableName);

            if (result == null)
                throw new Exception(string.Format("No {0} in environment variables", environmentVariableName));
            if (result.Length < minLength)
                throw new Exception(string.Format("Invalid {0} in environment variables", environmentVariableName));

            variableCache.Add(environmentVariableName, result);

            return result;
        }

        public static string GetOpenAiApiKey()
        {
            return GetEnvironmentVariable("OPENAI_API_KEY", 16);
        }
    }
}
