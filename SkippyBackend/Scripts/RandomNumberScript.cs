using ScriptRunner;
using System;

namespace CustomScripts
{
    public class RandomNumberScript : CompiledScript
    {
        public RandomNumberScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script generates and returns a random number between 1 and 100.
        /// </summary>
        /// <returns>A random number between 1 and 100</returns>
        [ScriptStart]
        public int GenerateRandomNumberForUser()
        {
            Random random = new Random();
            return random.Next(1, 101);
        }
    }
}