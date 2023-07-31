using System.Reflection;

namespace ScriptRunner.Models
{
    public class PreCompiledScript : ICompiledScriptContainer
    {
        public Type ScriptType { get; set; }

        private AttributeCommentProvider? attributeCommentProvider;

        public PreCompiledScript(Type scriptType)
        {
            ScriptType = scriptType;
        }

        public ICommentProvider? GetCommentProvider(MethodInfo method)
        {
            if (attributeCommentProvider == null)
                attributeCommentProvider = new AttributeCommentProvider(method);

            return attributeCommentProvider;
        }

        public CompiledScript GetCompiledScript(ScriptContext scriptContext)
        {
            CompiledScript? result = (CompiledScript?)Activator.CreateInstance(GetScriptType(), scriptContext);

            if (result == null)
                throw new Exception("Failed to create instance of script type " + GetScriptType().FullName);

            return result;
        }

        public Type GetScriptType()
        {
            return ScriptType;
        }
    }
}
