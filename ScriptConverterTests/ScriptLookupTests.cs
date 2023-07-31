using OpenAi.Models.Completion;
using ScriptConverter;
using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using ScriptRunner.Models;
using ScriptRunner.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptConverterTests
{
    [TestClass]
    public class ScriptLookupTests
    {
        [TestMethod]
        public void ScriptLookupWorksWithPrecompiledScripts()
        {
            FunctionScriptLookup lookup = new FunctionScriptLookup(new PreCompiledScriptProvider(typeof(TestScript)));

            lookup.LoadFunctionsAsync().Wait();

            Assert.IsTrue(lookup.TryGetCompiledScriptContainer("Run", out ICompiledScriptContainer scriptProvider));

            CompiledScript script = scriptProvider.GetCompiledScript(new ScriptContext());

            Assert.IsNotNull(script);
        }
    }

    public class TestScript : CompiledScript
    {
        public TestScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("This is a test script that multiplies two numbers and returns the result.")]
        [Parameter("factor1", "The first number to multiply")]
        [Parameter("factor2", "The second number to multiply")]
        [Parameter("userName", "The username of the user")]
        public object Run(int factor1, int factor2, string userName)
        {
            int result = factor1 * factor2;
            return $"{userName}: {result}";
        }
    }
}
