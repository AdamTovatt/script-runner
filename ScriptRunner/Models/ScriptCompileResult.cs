using System.Reflection;

namespace ScriptRunner.Models
{
    /// <summary>
    /// Represents the result of a script compilation
    /// </summary>
    public class ScriptCompileResult
    {
        /// <summary>
        /// The resulting assembly
        /// </summary>
        public Assembly? CompiledAssembly { get; set; }

        /// <summary>
        /// A list of errors that occured during compilation, will be null if everything went fine
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// A list of warnings that occured during compilation, will be null if everything went fine
        /// </summary>
        public List<string>? Warnings { get; set; }

        /// <summary>
        /// Will return the script from the CompiledAssembly. All scripts should inherit from CompiledScript and this method will return the first type that inherits from CompiledScript
        /// </summary>
        /// <param name="scriptContext">The context create the script in, the context can be used to provide the script with nice stuff</param>
        /// <returns>An instance of a class that inherits from CompiledScript (probably)</returns>
        /// <exception cref="Exception">Will throw an exception if no CompiledAssembly exists or if no type that inherits from CompiledScript was found</exception>
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