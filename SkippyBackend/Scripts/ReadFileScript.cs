using ScriptRunner;
using System.IO;

namespace CustomScripts
{
    public class ReadFileScript : CompiledScript
    {
        public ReadFileScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script reads the content of a file at the specified filePath and returns it as a string.
        /// </summary>
        /// <param name="filePath">The path of the file to read</param>
        [ScriptStart]
        public string ReadFileContent(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            
            return $"File not found at path: {filePath}";
        }
    }
}
