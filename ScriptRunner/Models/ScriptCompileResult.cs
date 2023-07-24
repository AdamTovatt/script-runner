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
        /// A dictionary containing the xml comments for the script, the key is the name of the member and it's parameters and the value is the xml comment
        /// </summary>
        public Dictionary<string, XmlComment>? XmlComments { get; set; }

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

            Type? type = GetScriptType();

            if (type != null)
            {
                CompiledScript? compiledScript = (CompiledScript?)Activator.CreateInstance(type, scriptContext);

                if (compiledScript != null)
                    return compiledScript;
            }

            throw new Exception("GetScript() was called on a ScriptCompileResult that has a CompiledAssembly but that assembly doesn't contain any type which is a type of CompiledScript. You should create a class that inherits from ScriptRunner.CompiledScript");
        }

        /// <summary>
        /// Will try to find a type that inherits from CompiledScript in the CompiledAssembly
        /// </summary>
        /// <returns>The type if it is found, null if it is not found</returns>
        /// <exception cref="Exception">Will throw an exception if the compiled assembly is null, this method should only be used when the compilation was successfull</exception>
        public Type? GetScriptType()
        {
            if (CompiledAssembly == null)
                throw new Exception("Tried to get script type from a ScriptCompileResult without any CompiledAssembly. Check for null on the CompiledAssembly before calling GetScript()!");

            foreach (Type type in CompiledAssembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(CompiledScript)))
                {
                    return type;
                }
            }

            return null;
        }

        public XmlComment? GetXmlComment(string methodHeader)
        {
            if(XmlComments == null)
                return null;

            if(XmlComments.TryGetValue(methodHeader, out XmlComment? xmlComment))
                return xmlComment;

            return null;
        }

        public string GetErrorMessages()
        {
            if(Errors == null)
                return string.Empty;

            return string.Join("\n", Errors);
        }
    }
}