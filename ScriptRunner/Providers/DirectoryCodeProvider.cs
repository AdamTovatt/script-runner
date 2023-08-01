using ScriptRunner.Helpers;
using ScriptRunner.Models;

namespace ScriptRunner.Providers
{
    public class DirectoryCodeProvider : ICodeProvider
    {
        public string DirectoryPath { get; set; }
        public string ScriptFileExtension { get; set; } = ".cs";

        public DirectoryCodeProvider(string directoryPath)
        {
            DirectoryPath = directoryPath;

            if (string.IsNullOrEmpty(directoryPath))
                DirectoryPath = Directory.GetCurrentDirectory();
        }

        public async Task<ScriptCode?> GetScriptAsync(string scriptName)
        {
            if (DirectoryPath.Length != 0 && !Directory.Exists(DirectoryPath))
                return null;

            string scriptPath;
            if (!scriptName.EndsWith(ScriptFileExtension))
                scriptPath = $"{Path.Combine(DirectoryPath, scriptName)}{ScriptFileExtension}";
            else
                scriptPath = $"{Path.Combine(DirectoryPath, scriptName)}";    

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

            if (compileResult.CompiledAssembly == null)
                throw new Exception($"Can't save script with compilation errors! The error messages are: {compileResult.GetErrorMessages()}");

            string scriptName = $"{compileResult.GetScriptType()?.Name ?? "invalidscript"}{ScriptFileExtension}";
            string scriptPath = Path.Combine(DirectoryPath, scriptName);

            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);

            await File.WriteAllTextAsync(scriptPath, scriptCode.Code);
        }

        public List<string> GetAllScriptNames()
        {
            List<string> result = new List<string>();

            if (DirectoryPath.Length != 0 && !Directory.Exists(DirectoryPath))
                return result;

            foreach (string file in Directory.GetFiles(DirectoryPath))
                result.Add(Path.GetFileName(file));

            return result;
        }

        public static DirectoryCodeProvider CreateFromRelativePath(string relativePath)
        {
            return new DirectoryCodeProvider(PathHelper.GetPathFromRelativePath(relativePath));
        }
    }
}
