namespace ScriptRunner.OpenAi.Models.Input.Types
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
