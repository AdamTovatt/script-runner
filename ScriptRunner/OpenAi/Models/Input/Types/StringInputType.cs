namespace ScriptRunner.OpenAi.Models.Input.Types
{
    public class StringInputType : InputType<string>, IInputType
    {
        public StringInputType(string value) : base(value) { }
    }
}
