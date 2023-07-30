using ScriptRunner;

namespace CustomScripts
{
    public class ImageUrlScript : CompiledScript
    {
        public ImageUrlScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script extracts the first image URL from an HTML string. Usefull if you have HTML and want to get an image.
        /// </summary>
        /// <param name="html">The HTML string.</param>
        [ScriptStart]
        public string GetImageUrlFromHtml(string html)
        {
            // Find the index of the first occurrence of the image tag
            int imgTagStartIndex = html.IndexOf("<img");

            if (imgTagStartIndex != -1)
            {
                // Find the index of the closing angle bracket of the image tag
                int imgTagEndIndex = html.IndexOf(">", imgTagStartIndex);

                if (imgTagEndIndex != -1)
                {
                    // Extract the image tag substring
                    string imgTag = html.Substring(imgTagStartIndex, imgTagEndIndex - imgTagStartIndex + 1);

                    // Find the index of the image URL within the image tag
                    int srcAttrStartIndex = imgTag.IndexOf("src=\"");

                    if (srcAttrStartIndex != -1)
                    {
                        // Find the index of the closing double quote of the src attribute
                        int srcAttrEndIndex = imgTag.IndexOf("\"", srcAttrStartIndex + 5);

                        if (srcAttrEndIndex != -1)
                        {
                            // Extract the image URL substring
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