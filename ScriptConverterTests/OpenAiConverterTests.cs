using ScriptConverter;
using ScriptRunner.Models;

namespace ScriptConverterTests
{
    [TestClass]
    public class OpenAiConverterTests
    {
        [TestMethod]
        public void ConvertFunction()
        {
            string script = @"using ScriptRunner;
                                
                              namespace Test
                              {
                                public class TestScript : CompiledScript
                                {
                                    public TestScript(ScriptContext context) : base(context) { }

                                    [ScriptStart]
                                    public object Run(int factor1, int factor2, string userName)
                                    {
                                        int result = factor1 * factor2;
                                        return $""{userName}: {result}"";
                                    }
                                }
                              }";

            ScriptCode code = new ScriptCode(script);
            ScriptCompileResult compileResult = code.Compile();

            Assert.IsNull(compileResult.Errors);
            Assert.IsNotNull(compileResult.CompiledAssembly);

            OpenAiScriptConverter.GetAsFunction(compileResult);
        }
    }
}