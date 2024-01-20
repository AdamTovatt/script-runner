namespace ScriptRunner.OpenAi.Models.Input
{
    public class InputResult
    {
        public InputInfo InputInfo { get; set; }
        public Type ValueType { get; set; }
        public object? Value { get; set; }

        public InputResult(InputInfo inputInfo, Type valueType, object? value)
        {
            InputInfo = inputInfo;
            ValueType = valueType;
            Value = value;
        }
    }
}
