using ScriptRunner.Models;
using ScriptRunner.Providers;

namespace ScriptRunnerTests
{
    [TestClass]
    public class DirectoryProviderTests
    {
        [TestMethod]
        public async Task SaveScript()
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

            DirectoryScriptProvider directory = DirectoryScriptProvider.CreateFromRelativePath("custom-scripts");
            await directory.SaveScriptAsync(new ScriptCode(script));

            ScriptCode? savedScript = await directory.GetScriptAsync("Test.TestScript");
            Assert.IsNotNull(savedScript);
            Assert.AreEqual(script, savedScript.Code);

            List<ScriptCode> scripts = await directory.GetAllScriptsAsync();
            Assert.IsNotNull(scripts);
            Assert.AreEqual(1, scripts.Count);

            Assert.AreEqual(1, directory.GetAllScriptNames().Count);
        }
    }
}
