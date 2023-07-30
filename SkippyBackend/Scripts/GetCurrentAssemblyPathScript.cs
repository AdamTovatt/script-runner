using ScriptRunner;
using System.IO;

namespace CustomScripts
{
    public class GetCurrentAssemblyPathScript : CompiledScript
    {
        public GetCurrentAssemblyPathScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script returns the file path of the currently executing assembly.
        /// </summary>
        [ScriptStart]
        public string GetCurrentAssemblyPath()
        {
            return System.Reflection.Assembly.GetEntryAssembly().Location;
        }
    }
}