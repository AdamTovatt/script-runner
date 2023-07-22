using System.Reflection;

namespace ScriptRunner.Models
{
    public class ScriptCompileResult
    {
        public Assembly? CompiledAssembly { get; set; }
        public List<string>? Errors { get; set; }
        public List<string>? Warnings { get; set; }

        public CompiledScript GetScript(ScriptContext scriptContext)
        {
            if (CompiledAssembly == null)
                throw new Exception("Tried to get script from a ScriptCompileResult without any CompiledAssembly. Check for null on the CompiledAssembly before calling GetScript()!");

            foreach (Type type in CompiledAssembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(CompiledScript)))
                {
                    CompiledScript? compiledScript = (CompiledScript?)Activator.CreateInstance(type, scriptContext);

                    if (compiledScript != null)
                    {
                        return compiledScript;
                    }
                }
            }

            throw new Exception("GetScript() was called on a ScriptCompileResult that has a CompiledAssembly but that assembly doesn't contain any type which is a type of CompiledScript. You should create a class that inherits from ScriptRunner.CompiledScript");
        }
    }
}