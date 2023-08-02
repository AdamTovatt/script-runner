namespace ScriptRunner.OpenAi.Models.InputTypes
{
    public class IntInputType : InputType<int>, IInputType
    {
        public IntInputType(int value) : base(value) { }
    }
}
