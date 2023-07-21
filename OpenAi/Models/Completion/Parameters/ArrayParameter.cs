namespace OpenAi.Models.Completion.Parameters
{
    public class ArrayParameter : Parameter
    {
        public ArrayParameter(string name, Type type, string description) : base(name, type, description) { }

        public List<Parameter>? Items { get; set; }

        public void AddParameter(Parameter parameter)
        {
            if (Items == null)
                Items = new List<Parameter>();

            Items.Add(parameter);
        }
    }
}
