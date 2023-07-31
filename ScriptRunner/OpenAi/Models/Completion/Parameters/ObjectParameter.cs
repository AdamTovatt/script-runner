using OpenAi.Models.Completion.Parameters;

namespace ScriptRunner.OpenAi.Models.Completion.Parameters
{
    public class ObjectParameter : Parameter
    {
        public ObjectParameter(string name, Type type, string description) : base(name, type, description) { }

        public List<Parameter>? Parameters { get; set; }

        public void AddParameter(Parameter parameter)
        {
            if (Parameters == null)
                Parameters = new List<Parameter>();

            Parameters.Add(parameter);
        }
    }
}
