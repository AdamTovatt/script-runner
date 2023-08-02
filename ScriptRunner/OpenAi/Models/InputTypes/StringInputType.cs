namespace ScriptRunner.OpenAi.Models.InputTypes
{
    public class StringInputType : InputType<string>, IInputType
    {
        public StringInputType(string value) : base(value) { }
    }
}
