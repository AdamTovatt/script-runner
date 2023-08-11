namespace ScriptRunner.OpenAi.Models.Input.Types
{
    public class IntegerInputType : InputType<long>, IInputType
    {
        public IntegerInputType(long value) : base(value) { }
    }
}
