namespace ScriptRunner.OpenAi.Models.InputTypes
{
    public class DecimalInputType : InputType<decimal>, IInputType
    {
        public DecimalInputType(decimal value) : base(value) { }
    }
}
