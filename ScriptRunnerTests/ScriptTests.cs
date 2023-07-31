using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using ScriptRunner.Models;
using ScriptRunner.Providers;
using System.Reflection;
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

        [TestMethod]
        public void UsePrecompiledScript()
        {
            PreCompiledScriptProvider scriptProvider1 = new PreCompiledScriptProvider(new PreCompiledScript(typeof(TestScript)), new PreCompiledScript(typeof(TestScript2)));
            PreCompiledScriptProvider scriptProvider2 = new PreCompiledScriptProvider(typeof(TestScript), typeof(TestScript2));

            List<ICompiledScriptContainer> scripts1 = scriptProvider1.GetCompiledScripts();
            List<ICompiledScriptContainer> scripts2 = scriptProvider2.GetCompiledScripts();

            Assert.IsNotNull(scripts1);
            Assert.IsNotNull(scripts2);

            Assert.AreEqual(2, scripts1.Count);
            Assert.AreEqual(2, scripts2.Count);

            Assert.AreEqual(typeof(TestScript), scripts1[0].GetScriptType());
            Assert.AreEqual(typeof(TestScript2), scripts1[1].GetScriptType());

            Assert.AreEqual(typeof(TestScript), scripts2[0].GetScriptType());
            Assert.AreEqual(typeof(TestScript2), scripts2[1].GetScriptType());

            ICompiledScriptContainer testScript = scripts2[0];
            ICommentProvider? commentProvider = testScript.GetCommentProvider(testScript.GetScriptType().GetMethods().First(x => x.GetCustomAttribute<ScriptStart>() != null));

            Assert.IsNotNull(commentProvider);
            Assert.AreEqual("This is a test script", commentProvider.Summary);
            Assert.AreEqual("The first number to multiply", commentProvider.GetParameterDescription("factor1"));
            Assert.AreEqual("The second number to multiply", commentProvider.GetParameterDescription("factor2"));
            Assert.AreEqual("The username of the user", commentProvider.GetParameterDescription("userName"));
        }
    }

    public class TestScript : CompiledScript
    {
        public TestScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Summary("This is a test script")]
        [Parameter("factor1", "The first number to multiply")]
        [Parameter("factor2", "The second number to multiply")]
        [Parameter("userName", "The username of the user")]
        public object Run(int factor1, int factor2, string userName)
        {
            int result = factor1 * factor2;
            return $"{userName}: {result}";
        }
    }

    public class TestScript2 : CompiledScript
    {
        public TestScript2(ScriptContext context) : base (context) { }

        [ScriptStart]
        [Summary("This is a hello world script")]
        public string Run()
        {
            return "Hello World!";
        }
    }
}