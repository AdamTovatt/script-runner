using ScriptRunner.OpenAi.Models.Completion;
using TiktokenSharp;

namespace ScriptRunner.OpenAi.Helpers
{
    public class TokenCounter
    {
        private static Dictionary<string, TikToken> counters = new Dictionary<string, TikToken>();

        private TikToken tikToken;

        public TokenCounter(string model)
        {
            if(counters.ContainsKey(model)) // check if the wanted model already exists, use it then
            {
                tikToken = counters[model];
                return;
            }

            try
            {
                tikToken = TikToken.EncodingForModel(model); // create a new model
                counters.Add(model, tikToken);
            }
            catch(NotImplementedException)
            {
                if (counters.ContainsKey(Model.DefaultTokenCounter)) // check if the wanted model already exists, use it then
                {
                    tikToken = counters[Model.DefaultTokenCounter];
                    return;
                }

                tikToken = TikToken.EncodingForModel(Model.DefaultTokenCounter); // couldn't create a new model for the wanted one, use default
                counters.Add(Model.DefaultTokenCounter, tikToken);
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
