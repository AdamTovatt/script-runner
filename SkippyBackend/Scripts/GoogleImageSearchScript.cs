using ScriptRunner;
using System;
using System.Net;

namespace CustomScripts
{
    public class GoogleImageSearchScript : CompiledScript
    {
        public GoogleImageSearchScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script retrieves the HTML from a Google image search based on the provided search term.
        /// </summary>
        /// <param name="searchTerm">The search term to use for the Google image search</param>
        [ScriptStart]
        public string GetGoogleImageSearchHtml(string searchTerm)
        {
            try
            {
                using (var client = new WebClient())
                {
                    string searchUrl = $"https://www.google.com/search?tbm=isch&q={Uri.EscapeDataString(searchTerm)}";
                    string html = client.DownloadString(searchUrl);
                    return html;
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
