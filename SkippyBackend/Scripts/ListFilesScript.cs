using ScriptRunner;
using System.IO;
using System.Collections.Generic;

namespace CustomScripts
{
    public class ListFilesScript : CompiledScript
    {
        public ListFilesScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script returns a list of all the files in a directory at the specified path.
        /// </summary>
        /// <param name="directoryPath">The path of the directory to list files from</param>
        [ScriptStart]
        public List<string> ListFiles(string directoryPath)
        {
            List<string> fileList = new List<string>();

            if (Directory.Exists(directoryPath))
            {
                string[] fileNames = Directory.GetFiles(directoryPath);
                foreach (string fileName in fileNames)
                {
                    fileList.Add(fileName);
                }
            }
            else
            {
                fileList.Add($"Directory not found at path: {directoryPath}");
            }

            return fileList;
        }
    }
}
