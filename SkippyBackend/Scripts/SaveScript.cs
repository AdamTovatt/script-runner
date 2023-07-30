using ScriptRunner;
using ScriptRunner.Models;
using ScriptRunner.Providers;
using System;

namespace CustomScripts
{
    public class SaveScript : CompiledScript
    {
        public SaveScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// Saves the provided script to the scripts directory. Usefull for when the user wants to create a new script.
        /// </summary>
        /// <param name="script">The content of the new script that should be saved</param>
        [ScriptStart]
        public string SaveScriptToFile(string script)
        {
            try
            {
                DirectoryCodeProvider directory = DirectoryCodeProvider.CreateFromRelativePath("scripts");

                directory.SaveScriptAsync(new ScriptCode(script)).Wait();

                return "The script was saved successfully";
            }
            catch (Exception exception)
            {
                return $"An error occured while saving the script: {exception.Message}";
            }
        }
    }
}