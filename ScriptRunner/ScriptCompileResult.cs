namespace ScriptRunner
{
    public class ScriptCompileResult
    {
        public CompiledScript? CompiledScript { get; set; }
        public List<string>? Errors { get; set; }
        public List<string>? Warnings { get; set; }
    }
}