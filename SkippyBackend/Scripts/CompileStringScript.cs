using System;
using ScriptRunner;
using ScriptRunner.Models;

namespace CustomScripts
{
    public class CompileStringScript : CompiledScript
    {
        public CompileStringScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script compiles a string and returns any errors or a happy message if there are no errors.
        /// </summary>
        /// <param name="scriptCode">The code string to compile</param>
        /// <returns>A string message indicating the result of the compilation</returns>
        [ScriptStart]
        public string CompileString(string scriptCode)
        {
            ScriptCode code = new ScriptCode(scriptCode);
            ScriptCompileResult compileResult = code.Compile();

            if (compileResult.Errors != null && compileResult.Errors.Count > 0)
            {
                return compileResult.GetErrorMessages();
            }
            else
            {
                return "Script compiled successfully!";
            }
        }
    }
}
