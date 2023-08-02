namespace ScriptRunner.OpenAi.Models.InputTypes
{
    public abstract class InputType<T>
    {
        public T Value { get; set; }

        public InputType(T value)
        {
            Value = value;
        }
    }
}
