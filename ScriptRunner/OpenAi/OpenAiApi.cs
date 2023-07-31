using OpenAi.Models.Completion;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using ScriptRunner.OpenAi.Models.Completion;

namespace ScriptRunner.OpenAi
{
    public class OpenAiApi
    {
        private const string completionUrl = "https://api.openai.com/v1/chat/completions";

        private string apiKey;
        private HttpClient client;

        public OpenAiApi(string apiKey, HttpClient client)
        {
            this.apiKey = apiKey;
            this.client = client;
        }

        public OpenAiApi(string apiKey)
        {
            this.apiKey = apiKey;
            client = new HttpClient();
        }

        /// <summary>
        /// Will complete in a conversation. Will use the inner most child conversations if there are any
        /// </summary>
        /// <param name="conversation">The conversation to complete</param>
        /// <param name="addResultToConversation">Wether or not the result of the completion should be added to the conversation straight away. The default is true and this is recommended since it will make sure that the result is added to the right conversation then</param>
        /// <param name="allowedFailCount">How many times the completion is allowed to fail before it will throw an exception. The default is 2. This means that if there is an error from open ai it will automatically try again</param>
        /// <returns></returns>
        public async Task<CompletionResult> CompleteAsync(Conversation conversation, bool addResultToConversation = true, int allowedFailCount = 2)
        {
            Conversation targetConversation = conversation;

            while(targetConversation.ChildConversation != null) // always take the inner most conversation
                targetConversation = targetConversation.ChildConversation;

            int failCount = 0;

            try
            {
                CompletionResult result = await CompleteAsync(targetConversation.CreateCompletionParameter());
                
                if (addResultToConversation)
                    targetConversation.Add(result);

                return result;
            }
            catch(Exception exception)
            {
                failCount++;
                Type exceptionType = exception.GetType();

                if ((exceptionType != typeof(HttpRequestException) && exceptionType != typeof(JsonException)) || failCount > allowedFailCount)
                    throw;

                return await CompleteAsync(targetConversation);
            }
        }

        private async Task<CompletionResult> CompleteAsync(CompletionParameter completionParameter)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, completionUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            string jsonContent = completionParameter.ToJson();
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    CompletionResult? result = JsonSerializer.Deserialize<CompletionResult>(await response.Content.ReadAsStringAsync());

                    if (result == null)
                        throw new NullReferenceException($"Deserialized CompletionResult was unexpectedly null when completing based on the completion parameter {completionParameter}");

                    return result;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    Thread.Sleep(1000);
                    return await CompleteAsync(completionParameter);
                }

                throw new CompletionException($"({response.StatusCode}) Error when completing based on the parameter {completionParameter}.\n{await response.Content.ReadAsStringAsync()}", response);
            }
            catch (TimeoutException)
            {
                throw new CompletionException("Error when creating an answer, the connection to OpenAi timed out.", null);
            }
        }
    }
}