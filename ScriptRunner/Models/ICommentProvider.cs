namespace ScriptRunner.Models
{
    public interface ICommentProvider
    {
        public string? Summary { get; set; }
        public Dictionary<string, string>? Parameters { get; set; }
        public string? Returns { get; set; }
        public string GetParameterDescription(string parameterName);
    }
}
