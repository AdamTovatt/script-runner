using ScriptRunner;
using ScriptRunner.Models;
using ScriptRunner.Providers;
using System;

namespace CustomScripts
{
    public class ReadScript : CompiledScript
    {
        public ReadScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// Will read a script from the scripts directory and return the script as a string. Usefull for when the user wants to read a script or of a script is needed for reference when creating a new script.
        /// </summary>
        /// <param name="scriptName">The name of the script to read from the scripts directory</param>
        [ScriptStart]
        public string ReadScriptFromFile(string scriptName)
        {
            try
            {
                DirectoryCodeProvider directory = DirectoryCodeProvider.CreateFromRelativePath("scripts");

                ScriptCode? scriptCode = directory.GetScriptAsync(scriptName).Result;

                if (scriptCode == null)
                {
                    return $"The script {scriptName} does not exist";
                }
                else
                {
                    return scriptCode.Code;
                }
            }
            catch (Exception exception)
            {
                return $"An error occured while reading the script: {exception.Message}";
            }
        }
    }
}