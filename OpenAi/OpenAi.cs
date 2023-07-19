using OpenAi.Models.Completion;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace OpenAi
{
    public class OpenAi
    {
        private const string completionUrl = "https://api.openai.com/v1/chat/completions";

        private string apiKey;
        private HttpClient client;

        public OpenAi(string apiKey, HttpClient client)
        {
            this.apiKey = apiKey;
            this.client = client;
        }

        public async Task<CompletionResult> CompleteAsync(CompletionParameter completionParameter)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, completionUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            string jsonContent = JsonSerializer.Serialize(completionParameter);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                CompletionResult? result = await JsonSerializer.DeserializeAsync<CompletionResult>(await response.Content.ReadAsStreamAsync());

                if (result == null)
                    throw new NullReferenceException($"Deserialized CompletionResult was unexpectedly null when completing based on the completion parameter {completionParameter}");

                return result;
            }

            throw new CompletionException($"Error when completing based on the parameter {completionParameter}", response);
        }
    }
}