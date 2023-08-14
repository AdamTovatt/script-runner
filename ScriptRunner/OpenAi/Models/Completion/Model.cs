namespace ScriptRunner.OpenAi.Models.Completion
{
    public class Model
    {
        public static string Default { get { return Gpt35Turbo; } }
        public static string DefaultTokenCounter { get { return Gpt35Turbo; } }

        public const string Gpt35Turbo = "gpt-3.5-turbo";
        public const string Gpt35Turbo16k = "gpt-3.5-turbo-16k";
    }
}
