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

            CompiledScript compiledScript = compileResult.GetScript(new ScriptContext());

            Dictionary<string, JsonNode> parameters = new Dictionary<string, JsonNode>();
            parameters.Add("factor1", 210);
            parameters.Add("factor2", 2);
            parameters.Add("userName", "Benim"!);

            object? result = compiledScript.Run(parameters);

            Assert.AreEqual("Benim: 420", result);
        }
    }
}