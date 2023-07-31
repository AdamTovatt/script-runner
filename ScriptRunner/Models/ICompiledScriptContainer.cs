using System.Reflection;

namespace ScriptRunner.Models
{
    public interface ICompiledScriptContainer
    {
        public CompiledScript GetCompiledScript(ScriptContext scriptContext);
        public Type GetScriptType();
        public ICommentProvider? GetCommentProvider(MethodInfo method);
    }
}
