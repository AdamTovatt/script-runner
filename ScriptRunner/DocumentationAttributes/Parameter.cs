namespace ScriptRunner.DocumentationAttributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class Parameter : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Parameter(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
