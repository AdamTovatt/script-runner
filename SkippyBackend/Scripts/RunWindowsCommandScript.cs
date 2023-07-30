using ScriptRunner;
using System;
using System.Diagnostics;

namespace CustomScripts
{
    public class RunWindowsCommandScript : CompiledScript
    {
        public RunWindowsCommandScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// Runs a Windows command provided as a string and returns the output.
        /// </summary>
        /// <param name="command">The Windows command to run</param>
        /// <returns>The output of the command</returns>
        [ScriptStart]
        public string RunWindowsCommand(string command)
        {
            try
            {
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C {command}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return output;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
