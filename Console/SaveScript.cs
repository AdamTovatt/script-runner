using ScriptRunner;
using ScriptRunner.Models;
using ScriptRunner.Providers;

namespace CustomScripts
{
    public class SaveScriptExample : CompiledScript
    {
        public SaveScriptExample(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script saves the provided content as a new script file in a specified directory.
        /// </summary>
        /// <param name="content">The content of the new script</param>
        [ScriptStart]
        public string SaveScriptToFile(string content)
        {
            try
            {
                DirectoryScriptProvider directory = DirectoryScriptProvider.CreateFromRelativePath("scripts");

                directory.SaveScriptAsync(new ScriptCode(content)).Wait();

                return "The script was saved successfully";
            }
            catch (Exception exception)
            {
                return $"An error occured while saving the script: {exception.Message}";
            }
        }
    }
}