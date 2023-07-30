using System;
using System.IO;
using System.Net;
using ScriptRunner;

namespace CustomScripts
{
    public class DownloadImageScript : CompiledScript
    {
        public DownloadImageScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script downloads an image from the specified URL and saves it to the specified file path.
        /// </summary>
        /// <param name="url">The URL of the image to download</param>
        /// <param name="filePath">The file path to save the image</param>
        [ScriptStart]
        public object DownloadAndSaveImage(string url, string filePath)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, filePath);
                }
                return $"Image downloaded and saved successfully to {filePath}";
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }
    }
}
