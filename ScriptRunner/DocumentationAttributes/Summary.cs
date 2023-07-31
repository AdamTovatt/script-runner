namespace ScriptRunner.DocumentationAttributes
{
    public class Summary : Attribute
    {
        public string Text { get; set; }

        public Summary(string text)
        {
            Text = text;
        }
    }
}
