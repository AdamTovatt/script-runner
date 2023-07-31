using ScriptRunner.Models;

namespace ScriptRunner.Providers
{
    public interface ICompiledScriptProvider : ICodeOrScriptProvider
    {
        public List<ICompiledScriptContainer> GetCompiledScripts();
    }
}
