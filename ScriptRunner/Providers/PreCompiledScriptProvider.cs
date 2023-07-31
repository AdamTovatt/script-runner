using ScriptRunner.Models;

namespace ScriptRunner.Providers
{
    public class PreCompiledScriptProvider : ICompiledScriptProvider
    {
        public List<ICompiledScriptContainer> CompiledScriptContainers { get; set; }

        public PreCompiledScriptProvider(params ICompiledScriptContainer[] compiledScripts)
        {
            CompiledScriptContainers = compiledScripts.ToList();
        }

        public PreCompiledScriptProvider(params Type[] scriptTypes)
        {
            List<ICompiledScriptContainer> containers = new List<ICompiledScriptContainer>();

            foreach (Type type in scriptTypes)
            {
                if (!typeof(CompiledScript).IsAssignableFrom(type))
                    throw new Exception($"Tried to create a pre compiled script provider with a type that is not a type of compiled script: {type.Name}");

                containers.Add(new PreCompiledScript(type));
            }

            CompiledScriptContainers = containers;
        }

        public void AddScript(ICompiledScriptContainer compiledScript)
        {
            CompiledScriptContainers.Add(compiledScript);
        }

        public List<ICompiledScriptContainer> GetCompiledScripts()
        {
            return CompiledScriptContainers;
        }
    }
}
