using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using ScriptRunner.OpenAi.Models.Input;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkippyBackend.PrecompiledScripts
{
    public class GetFlightsScript : CompiledScript
    {
        private const string cookie = "TrackingId=3c98008a-1002-43fe-9c89-59ba499d87dc; reese84=3:5Tpq2anwEm3dVSxFO8x5CQ==:k48r9t3sNwdDEIwAtnfcOH2BTcOM2ap79GlkYy1L6PKRo7fGq7rQwxTqv4E6Eo4hBc3NXvgXkLYybH9Ghi3sWKzxLjMheU5pOz/xPdft0MIYIeguzDGM/ytxP1xD7ixyTC+evPBUKnHBKv6t7XUa2pC3dzPUtu6yx5cSmkUX3xMNnP7xZI3YvdN6o5hZJGIdQHNRrYPFuvfuxm0zklMIp4TDsqetQ/E/XWMH26CjFZrWlm/fRp8SFF+Iu72al/Qfwts7AfJmcmUaPmtpa5zBp3JFA3ahGv1io0jhltu+e3g4AefqxKmwf2hhq6OqKz6n7fIZv2Sd2J4sRRUG5STv0mrnvLkkf4Y7qXuuUEXpia2fp8896rGMBD0Cvb5FWbloliU3qxHko7+SlKnbMdconILcF1nAoNYF6eVYa7nTzMQ+sdFL3Hd17O59zQ2PpkX9GRxFeArQg4IJojxXqnjwAg==:rEsWM9m3IXfjhI4v/poC9D27QjkcT1O5geh95whHc+Y=; _cookienew=acknowledged; _cookiepersonalization=true; _cookieanalytics=true; _sas_randomize=13; _gcl_au=1.1.228880344.1690278127; _scid=c61b7de0-879c-4be4-8df1-a69ba67dafb0; _ga_4BBTM6PGF2=GS1.1.1691798815.4.0.1691798817.0.0.0; _ga=GA1.2.202348228.1690278128; FPID=FPID2.2.mu1XMNFMVl5vT8vAndBuQwC4cdjs9vCXLLcfLYrw8eg%3D.1690278128; _gid=GA1.2.572698985.1691790419; FPLC=5U4maOHz9dlXfm%2BY24z1ZD8eUkQ8%2Bk%2B4ZR7T8LQnaiFAWbTe95nS94rm5pfSUFi%2B5kPNSuNk1seePdEG%2FMQyrZWUbsUXPAvRcdX4m1WPXDncAH7mEQzBiEvSrjsT6g%3D%3D; ASLBSA=0003a231a57caabde4e0aefdbf30bb2a992631e1fc64078c9a056deaf3caa3edeff4; ASLBSACORS=0003a231a57caabde4e0aefdbf30bb2a992631e1fc64078c9a056deaf3caa3edeff4; ln_or=eyIzNDczMjk4IjoiZCJ9; GTM-inMarketFlag=in-market; _scid_r=c61b7de0-879c-4be4-8df1-a69ba67dafb0; _uetsid=9b0e4b50389011eea69315c4fc88e401; _uetvid=85219f102acf11eea83967b12eb073dc";

        private HttpClient httpClient = new HttpClient();

        public GetFlightsScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("Will get flights from a given city to a given other city on a specified date")]
        [Parameter("from", "The city to search for flight from")]
        [Parameter("to", "The city to search for flights to")]
        public async Task<string> GetFlights(string from, string to)
        {
            httpClient.DefaultRequestHeaders.Add("Cookie", cookie);

            HttpResponseMessage countryDataResponse = await httpClient.GetAsync("https://components.flysas.com/book-api-cepfiles/countryinfodata");

            if (!countryDataResponse.IsSuccessStatusCode)
                return "There was an error when getting the list of available cities";

            CountryInfo? countryInfo = CountryInfo.FromJson(await countryDataResponse.Content.ReadAsStringAsync());

            if (countryInfo == null) return "There was an error when deserializing the country info api result";

            string? fromCode = await GetAirportCodeForLocation(from, countryInfo);

            if (fromCode == null)
                return $"I could not find the location you want to fly from ({from})"; 

            string? toCode = await GetAirportCodeForLocation(to, countryInfo);

            if (toCode == null)
                return $"I could not find the location you want to fly to ({to})";

            if (!(await Context.Conversation.Input.GetAsync<bool?>($"So you want to fly from {from} ({fromCode}) to {to} ({toCode})?", true)).Value)
                return "User said they didn't want to fly to the entered places, they have to start over if they want to change places";

            DateTime date = DateTime.MinValue;
            string dateInputMessage = "When do you want to fly?";

            while (date == DateTime.MinValue)
            {
                string inputChoice = (await Context.Conversation.Input.GetAsync<string>(dateInputMessage, choices: GetTimeChoices()))!;

                if (!DateParser.TryParseDate(inputChoice, out date))
                    dateInputMessage = "That's not a date that I can understand. Please answer with only the date of the current month like \"12\" or with the full date in the format yyyy-MM-dd";
            }

            int numberOfPassengers = (await Context.Conversation.Input.GetAsync<int?>("How many passengers are you?", true))!.Value;

            HttpResponseMessage offersResponse = await httpClient.GetAsync($"https://www.flysas.com/api/offers/flights/?to={toCode}&from={fromCode}&outDate={date.ToString("yyyyMMdd")}&yth={numberOfPassengers}&channel=web&cepId=YOUTH");

            if(!offersResponse.IsSuccessStatusCode)
                return "There was an error when getting the list of available flights";

            string offersJson = await offersResponse.Content.ReadAsStringAsync();

            return "not implemented";
        }

        private async Task<string?> GetAirportCodeForLocation(string location, CountryInfo countryInfo)
        {
            string? airportCode = countryInfo.GetAirportCodeByCityName(location);

            if(airportCode == null)
            {
                List<Origin>? origins = countryInfo.GetOriginsByCountryName(location);

                if (origins == null) return null;

                List<InputChoice> choices = new List<InputChoice>();
                origins.ForEach(origin =>
                {
                    choices.Add(new InputChoice(origin.AirportCode, origin.CityName?.Trim() ?? origin?.AirportCode?.Trim() ?? "invalid airport"));
                });

                InputChoice inputChoice = (await Context.Conversation.Input.GetAsync<InputChoice?>($"What airport do you want to use in {location}?", true, choices: choices))!;

                if (inputChoice.Value == null) return null;

                airportCode = (string)inputChoice.Value;
            }

            return airportCode;
        }

        private List<InputChoice> GetTimeChoices()
        {
            List<InputChoice> dateChoices = new List<InputChoice>()
            {
                new InputChoice(DateTime.Now, "Today"),
                new InputChoice(DateTime.Now + TimeSpan.FromDays(1), "Tomorrow"),
                new InputChoice(DateTime.Now + TimeSpan.FromDays(2), "In two days"),
            };

            return dateChoices;
        }

        public class DateParser
        {
            public static bool TryParseDate(string input, out DateTime parsedDate)
            {
                if (input.Equals("Today", StringComparison.OrdinalIgnoreCase))
                {
                    parsedDate = DateTime.Today;
                    return true;
                }
                else if (input.Equals("Tomorrow", StringComparison.OrdinalIgnoreCase))
                {
                    parsedDate = DateTime.Today.AddDays(1);
                    return true;
                }
                else if (input.Equals("In two days", StringComparison.OrdinalIgnoreCase))
                {
                    parsedDate = DateTime.Today.AddDays(2);
                    return true;
                }
                else if (int.TryParse(input, out int day))
                {
                    // If the input is a number, assume it's a day of the current month
                    DateTime now = DateTime.Now;
                    parsedDate = new DateTime(now.Year, now.Month, day);
                    return true;
                }
                else if (DateTime.TryParseExact(input, "yyyy-MM-dd", null, DateTimeStyles.None, out parsedDate))
                {
                    // If the input matches the format yyyy-MM-dd, parse it directly
                    return true;
                }
                else
                {
                    parsedDate = DateTime.MinValue;
                    return false;
                }
            }
        }

        public partial class CountryInfo
        {
            [JsonPropertyName("sites")]
            public List<Site>? Sites { get; set; }

            public static CountryInfo? FromJson(string json)
            {
                return JsonSerializer.Deserialize<CountryInfo>(json);
            }

            public string? GetAirportCodeByCityName(string city)
            {
                if (Sites == null) return null;
                Site? site = Sites.FirstOrDefault(s => s.Origins != null && s.Origins.Any(origin => origin.CityName != null && origin.CityName.ToLower().Trim() == city.ToLower().Trim()));
            
                if(site == null || site.Origins == null) return null;

                Origin? origin = site.Origins.FirstOrDefault(origin => origin.CityName != null && origin.CityName.ToLower().Trim() == city.ToLower().Trim());

                if (origin == null) return null;

                return origin.AirportCode;
            }

            public List<Origin>? GetOriginsByCountryName(string countryName)
            {
                if (Sites == null) return null;

                Site? resultSite = Sites.FirstOrDefault(site => site.CountryName != null && site.CountryName.ToLower().Trim() == countryName.ToLower().Trim());

                if(resultSite == null) return null;

                return resultSite.Origins;
            }
        }

        public partial class Site
        {
            [JsonPropertyName("countryCode")]
            public string? CountryCode { get; set; }

            [JsonPropertyName("countryName")]
            public string? CountryName { get; set; }

            [JsonPropertyName("imagePath")]
            public Uri? ImagePath { get; set; }

            [JsonPropertyName("languages")]
            public List<Language>? Languages { get; set; }

            [JsonPropertyName("origins")]
            public List<Origin>? Origins { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("displayCurrencyCode")]
            public string? DisplayCurrencyCode { get; set; }
        }

        public partial class Language
        {
            [JsonPropertyName("languageCode")]
            public string? LanguageCode { get; set; }

            [JsonPropertyName("language")]
            public string? LanguageLanguage { get; set; }

            [JsonPropertyName("homePath")]
            public Uri? HomePath { get; set; }
        }

        public partial class Origin
        {
            [JsonPropertyName("airportCode")]
            public string? AirportCode { get; set; }

            [JsonPropertyName("cityName")]
            public string? CityName { get; set; }
        }
    }
}
