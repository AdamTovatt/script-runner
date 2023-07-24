using ScriptRunner.Models;

namespace ScriptRunner.Providers
{
    public class DirectoryScriptProvider : IScriptProvider
    {
        public string DirectoryPath { get; set; }
        public string ScriptFileExtension { get; set; } = ".cs";

        public DirectoryScriptProvider(string directoryPath)
        {
            DirectoryPath = directoryPath;

            if (string.IsNullOrEmpty(directoryPath))
                DirectoryPath = Directory.GetCurrentDirectory();
        }

        public async Task<ScriptCode?> GetScriptAsync(string scriptName)
        {
            if (DirectoryPath.Length != 0 && !Directory.Exists(DirectoryPath))
                return null;

            string scriptPath = $"{Path.Combine(DirectoryPath, scriptName)}{ScriptFileExtension}";

            if (!File.Exists(scriptPath))
                return null;

            return new ScriptCode(await File.ReadAllTextAsync(scriptPath));
        }

        public async Task<List<ScriptCode>> GetAllScriptsAsync()
        {
            List<ScriptCode> result = new List<ScriptCode>();

            if (DirectoryPath.Length != 0 && !Directory.Exists(DirectoryPath))
                return result;

            foreach (string file in Directory.GetFiles(DirectoryPath))
            {
                if (file.EndsWith(ScriptFileExtension))
                {
                    result.Add(new ScriptCode(await File.ReadAllTextAsync(file)));
                }
            }

            return result;
        }

        public async Task SaveScriptAsync(ScriptCode scriptCode)
        {
            ScriptCompileResult compileResult = scriptCode.Compile();

            if (compileResult.CompiledAssembly != null)
            {
                string scriptName = $"{compileResult.GetScriptType()}{ScriptFileExtension}";
                string scriptPath = Path.Combine(DirectoryPath, scriptName);

                if(!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);

                await File.WriteAllTextAsync(scriptPath, scriptCode.Code);
            }
        }

        public static DirectoryScriptProvider CreateFromRelativePath(string relativePath)
        {
            return new DirectoryScriptProvider(Path.Combine(Directory.GetCurrentDirectory(), relativePath));
        }
    }
}
