namespace ScriptRunner.OpenAi.Models.Input.Types
{
    public class DecimalInputType : InputType<decimal>, IInputType
    {
        public DecimalInputType(decimal value) : base(value) { }
    }
}
