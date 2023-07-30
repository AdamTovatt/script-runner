using ScriptRunner;
using System.Collections.Generic;
using System.Diagnostics;

namespace CustomScripts
{
    public class ProcessNamesScript : CompiledScript
    {
        public ProcessNamesScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This function will return a list of the names of all the currently running processes.
        /// </summary>
        [ScriptStart]
        public List<string> GetProcessNames()
        {
            List<string> processNames = new List<string>();
            
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                processNames.Add(process.ProcessName);
            }

            return processNames;
        }
    }
}
