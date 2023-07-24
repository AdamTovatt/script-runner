using OpenAi.Models.Completion;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace OpenAi
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

        public async Task<CompletionResult> CompleteAsync(CompletionParameter completionParameter)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, completionUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            string jsonContent = completionParameter.ToJson();
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

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
    }
}