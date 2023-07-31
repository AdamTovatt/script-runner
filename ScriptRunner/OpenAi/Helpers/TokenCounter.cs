using OpenAi.Models.Completion;
using ScriptRunner.OpenAi.Models.Completion;
using TiktokenSharp;

namespace ScriptRunner.OpenAi.Helpers
{
    public class TokenCounter
    {
        private TikToken tikToken;

        public TokenCounter(string model)
        {
            try
            {
                tikToken = TikToken.EncodingForModel(model);
            }
            catch(NotImplementedException)
            {
                tikToken = TikToken.EncodingForModel(Model.Gpt35Turbo);
            }
        }

        public int GetTokenCount(Conversation conversation)
        {
            string json = conversation.CreateCompletionParameter().ToJson();
            List<int> encoded = tikToken.Encode(json);
            return encoded.Count;
        }
    }
}
