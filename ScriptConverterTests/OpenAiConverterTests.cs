using OpenAi.Models.Completion;
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
                                /// <summary>
                                /// This is a test script class
                                /// </summary>
                                public class TestScript : CompiledScript
                                {
                                    public TestScript(ScriptContext context) : base(context) { }

                                    /// <summary>
                                    /// This is a test script that multiplies two numbers and returns the result.
                                    /// </summary>
                                    /// <param name=""factor1"">The first number to multiply</param>
                                    /// <param name=""factor2"">The second number to multiply</param>
                                    /// <param name=""userName"">The username of the user</param>
                                    /// <returns>A string</returns>
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

            Function function = OpenAiScriptConverter.GetAsFunction(compileResult);
        }
    }
}