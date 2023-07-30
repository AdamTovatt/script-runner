using ScriptRunner;
using System;

namespace CustomScripts
{
    public class TimeScript : CompiledScript
    {
        public TimeScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script tells the current time.
        /// </summary>
        [ScriptStart]
        public string GetCurrentTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
