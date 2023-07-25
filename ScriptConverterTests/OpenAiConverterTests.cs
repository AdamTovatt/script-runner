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

            Assert.AreEqual("This is a test script that multiplies two numbers and returns the result.", function.Description);
            Assert.AreEqual("Run", function.Name);
            Assert.IsNotNull(function.Parameters);
            Assert.IsNotNull(function.Parameters.Parameters);
            Assert.AreEqual(3, function.Parameters.Parameters.Count);
            Assert.AreEqual("factor1", function.Parameters.Parameters[0].Name);
            Assert.AreEqual("The first number to multiply", function.Parameters.Parameters[0].Description);
            Assert.AreEqual("factor2", function.Parameters.Parameters[1].Name);
            Assert.AreEqual("The second number to multiply", function.Parameters.Parameters[1].Description);
            Assert.AreEqual("userName", function.Parameters.Parameters[2].Name);
            Assert.AreEqual("The username of the user", function.Parameters.Parameters[2].Description);
        }
    }
}