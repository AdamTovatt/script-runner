using ScriptRunner;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CustomScripts
{
    public class NumberInfoScript : CompiledScript
    {
        public NumberInfoScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script retrieves information about a number from the Numbers API.
        /// </summary>
        /// <param name="number">The number to get information about</param>
        [ScriptStart]
        public async Task<string> GetNumberInfo(int number)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"http://numbersapi.com/{number}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                return responseBody;
            }
        }
    }
}