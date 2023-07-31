using ScriptRunner;
using ScriptRunner.Models;
using System.Text.Json.Nodes;

namespace ScriptRunnerTests
{
    [TestClass]
    public class ScriptTests
    {
        [TestMethod]
        public void CompileAndRunScript()
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
            Assert.IsNotNull(compileResult.XmlComments);
            Assert.IsNotNull(compileResult.XmlComments["Run(int factor1, int factor2, string userName)"]);
            Assert.IsNotNull(compileResult.XmlComments["Run(int factor1, int factor2, string userName)"].Parameters);
            Assert.AreEqual(compileResult.XmlComments["Run(int factor1, int factor2, string userName)"].Summary, "This is a test script that multiplies two numbers and returns the result.");
            Assert.AreEqual(3, compileResult.XmlComments["Run(int factor1, int factor2, string userName)"].Parameters?.Count());

            CompiledScript compiledScript = compileResult.GetCompiledScript(new ScriptContext());

            Dictionary<string, JsonNode> parameters = new Dictionary<string, JsonNode>();
            parameters.Add("factor1", 210);
            parameters.Add("factor2", 2);
            parameters.Add("userName", "Benim"!);

            object? result = compiledScript.Run(parameters);

            Assert.AreEqual("Benim: 420", result);
        }
    }
}