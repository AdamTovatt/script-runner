using ScriptRunner;
using System.Diagnostics;
using System;

namespace CustomScripts
{
    public class StartFileScript : CompiledScript
    {
        public StartFileScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script starts a file by the provided file path. 
        /// This function is useful for opening files or images.
        /// </summary>
        /// <param name="filePath">The path of the file to start</param>
        [ScriptStart]
        public string StartFile(string filePath)
        {
            try
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                };
                process.Start();
            }
            catch (Exception exception)
            {
                return $"An error occured while starting the file: {exception.Message}";
            }

            return "The file was started successfully!";
        }
    }
}
