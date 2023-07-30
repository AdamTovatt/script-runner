using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ScriptRunner;

namespace CustomScripts
{
    public class KanyeQuoteScript : CompiledScript
    {
        public KanyeQuoteScript(ScriptContext context) : base(context) { }

        public class KanyeQuoteResponse
        {
            public string Quote { get; set; }
        }

        /// <summary>
        /// This script retrieves a specified number of quotes from Kanye West's REST API.
        /// </summary>
        /// <param name="count">The number of quotes to retrieve</param>
        [ScriptStart]
        public async Task<string[]> GetKanyeQuotes(int count)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://api.kanye.rest/";
                string[] quotes = new string[count];

                for (int i = 0; i < count; i++)
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        var quoteResponse = JsonSerializer.Deserialize<KanyeQuoteResponse>(json, options);
                        quotes[i] = quoteResponse.Quote;
                    }
                    else
                    {
                        throw new Exception("Failed to retrieve Kanye quote");
                    }
                }

                return quotes;
            }
        }
    }
}