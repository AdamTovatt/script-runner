﻿using ScriptRunner.OpenAi.Models.Completion;
using TiktokenSharp;

namespace ScriptRunner.OpenAi.Helpers
{
    public class TokenCounter
    {
        private static Dictionary<string, TikToken> counters = new Dictionary<string, TikToken>();

        private TikToken tikToken;
        private static object counterLock = new object();

        public TokenCounter(string model)
        {
            if (counters.ContainsKey(model)) // check if the wanted model already exists, use it then
            {
                tikToken = counters[model];
                return;
            }

            try
            {
                tikToken = TikToken.EncodingForModel(model); // create a new model
                AddCounter(model, tikToken);
            }
            catch (NotImplementedException)
            {
                if (counters.ContainsKey(Model.DefaultTokenCounter)) // check if the wanted model already exists, use it then
                {
                    tikToken = counters[Model.DefaultTokenCounter];
                    return;
                }

                tikToken = TikToken.EncodingForModel(Model.DefaultTokenCounter); // couldn't create a new model for the wanted one, use default
                AddCounter(Model.DefaultTokenCounter, tikToken);
            }
        }

        private void AddCounter(string name, TikToken counter)
        {
            lock (counterLock)
            {
                if (!counters.ContainsKey(name))
                    counters.Add(name, counter);
            }
        }

        public int GetTokenCount(Conversation conversation)
        {
            string json = conversation.CreateCompletionParameter().ToJson();
            List<int> encoded = tikToken.Encode(json);
            return encoded.Count;
        }

        public int GetTokenCount(string text)
        {
            return tikToken.Encode(text).Count;
        }
    }
}
