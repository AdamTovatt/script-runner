namespace ScriptRunner.Providers
{
    public class NamespaceProvider
    {
        public static NamespaceProvider Instance { get { if (_instance == null) _instance = new NamespaceProvider(); return _instance; } }
        private static NamespaceProvider? _instance;

        private List<string> defaultNameSpaces;

        public NamespaceProvider()
        {
            defaultNameSpaces = new List<string>
            {
                "System",
                "System.Net",
                "System.Linq",
                "System.Text",
                "System.Collections.Generic"
            };
        }

        public List<string> GetDefault()
        {
            return defaultNameSpaces;
        }
    }
}
