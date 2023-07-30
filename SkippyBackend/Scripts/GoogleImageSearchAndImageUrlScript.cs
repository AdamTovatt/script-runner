using System;
using System.Net;
using ScriptRunner;

namespace CustomScripts
{
    public class GoogleImageSearchAndImageUrlScript : CompiledScript
    {
        public GoogleImageSearchAndImageUrlScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script retrieves the HTML from a Google image search based on the provided search term
        /// and extracts the first image URL from the HTML. Useful if you want to perform a google image search.
        /// </summary>
        /// <param name="searchTerm">The search term to use for the Google image search</param>
        [ScriptStart]
        public string[] SearchAndExtractImageUrl(string searchTerm)
        {
            string searchHtml = GetGoogleImageSearchHtml(searchTerm);
            string imageUrl = GetImageUrlFromHtml(searchHtml);

            return imageUrl != null ? new string[] { imageUrl } : new string[] { };
        }

        private string GetGoogleImageSearchHtml(string searchTerm)
        {
            try
            {
                using (WebClient client = new WebClient())
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

        private string GetImageUrlFromHtml(string html)
        {
            int imgTagStartIndex = html.IndexOf("<img");

            if (imgTagStartIndex != -1)
            {
                int imgTagEndIndex = html.IndexOf(">", imgTagStartIndex);

                if (imgTagEndIndex != -1)
                {
                    string imgTag = html.Substring(imgTagStartIndex, imgTagEndIndex - imgTagStartIndex + 1);

                    int srcAttrStartIndex = imgTag.IndexOf("src=\"");

                    if (srcAttrStartIndex != -1)
                    {
                        int srcAttrEndIndex = imgTag.IndexOf("\"", srcAttrStartIndex + 5);

                        if (srcAttrEndIndex != -1)
                        {
                            string imageUrl = imgTag.Substring(srcAttrStartIndex + 5, srcAttrEndIndex - srcAttrStartIndex - 5);
                            return imageUrl;
                        }
                    }
                }
            }

            return null;
        }
    }
}