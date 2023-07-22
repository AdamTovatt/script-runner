namespace ScriptRunner
{
    public class CompiledScript
    {
        public ScriptContext Context { get; set; }

        public CompiledScript(ScriptContext context)
        {
            Context = context;
        }
    }
}
