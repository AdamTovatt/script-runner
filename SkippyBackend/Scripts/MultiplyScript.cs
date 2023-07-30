using ScriptRunner;

namespace CustomScripts
{
    public class MultiplyScript : CompiledScript
    {
        public MultiplyScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This is a test script that multiplies two numbers and returns the result. It also takes a username as a parameter.
        /// </summary>
        /// <param name="factor1">The first number to multiply</param>
        /// <param name="factor2">The second number to multiply</param>
        [ScriptStart]
        public object MultiplyTwoNumbersForUser(int factor1, int factor2)
        {
            return factor1 * factor2;
        }
    }
}