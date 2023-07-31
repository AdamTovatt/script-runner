namespace ScriptRunner.ScriptConvertion
{
    public class ConvertionException : Exception
    {
        public List<string> Errors { get; set; }

        public ConvertionException(List<string> errors) : base("One or more compilation errors occured when converting scripts to functions")
        {
            Errors = errors;
        }
    }
}
