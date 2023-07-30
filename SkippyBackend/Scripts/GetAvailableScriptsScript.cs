using ScriptRunner;
using ScriptRunner.Models;
using ScriptRunner.Providers;
using System;

namespace CustomScripts
{
    public class GetAvailableScriptsScript : CompiledScript
    {
        public GetAvailableScriptsScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// Will return a list of the names for all available scripts in the scripts directory. Usefull for when you want a script to look at as a reference when creating a new script but don't know what scripts are available, this function will return a list of all available scripts.
        /// </summary>
        [ScriptStart]
        public string GetAvailableScripts()
        {
            try
            {
                DirectoryCodeProvider directory = DirectoryCodeProvider.CreateFromRelativePath("scripts");

                return string.Join(", ", directory.GetAllScriptNames());
            }
            catch (Exception exception)
            {
                return $"An error occured while reading the script: {exception.Message}";
            }
        }
    }
}