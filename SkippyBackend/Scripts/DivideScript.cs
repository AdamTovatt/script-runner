using ScriptRunner;

namespace CustomScripts
{
    public class DivideScript : CompiledScript
    {
        public DivideScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This is a test script that divides two numbers and returns the result.
        /// </summary>
        /// <param name="dividend">The number to be divided</param>
        /// <param name="divisor">The number to divide by</param>
        [ScriptStart]
        public double DivideTwoNumbersForUser(double dividend, double divisor)
        {
            return dividend / divisor;
        }
    }
}