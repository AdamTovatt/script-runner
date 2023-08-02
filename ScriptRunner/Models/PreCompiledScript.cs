using System.Reflection;

namespace ScriptRunner.Models
{
    public class PreCompiledScript : ICompiledScriptContainer
    {
        public Type ScriptType { get; set; }

        private AttributeDocumentationProvider? attributeDocumentationProvider;

        public PreCompiledScript(Type scriptType)
        {
            ScriptType = scriptType;
        }

        public IDocumentationProvider? GetDocumentationProvider(MethodInfo method)
        {
            if (attributeDocumentationProvider == null)
                attributeDocumentationProvider = new AttributeDocumentationProvider(method);

            return attributeDocumentationProvider;
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
