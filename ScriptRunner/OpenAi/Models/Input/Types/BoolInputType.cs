namespace ScriptRunner.OpenAi.Models.Input.Types
{
    public class BoolInputType : InputType<bool>, IInputType
    {
        public BoolInputType(bool value) : base(value) { }
    }
}
