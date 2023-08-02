namespace ScriptRunner.OpenAi.Models.InputTypes
{
    public class BoolInputType : InputType<bool>, IInputType
    {
        public BoolInputType(bool value) : base(value) { }
    }
}
