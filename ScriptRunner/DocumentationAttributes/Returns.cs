namespace ScriptRunner.DocumentationAttributes
{
    public class Returns : Attribute
    {
        public string Text { get; set; }

        public Returns(string text)
        {
            Text = text;
        }
    }
}
