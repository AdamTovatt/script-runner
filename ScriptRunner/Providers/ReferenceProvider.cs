using Microsoft.CodeAnalysis;
using System.Reflection;

namespace ScriptRunner.Providers
{
    public class ReferenceProvider
    {
        public static ReferenceProvider Instance { get { if (_instance == null) _instance = new ReferenceProvider(); return _instance; } }
        private static ReferenceProvider? _instance;

        private List<MetadataReference> defaultReferences;

        public ReferenceProvider()
        {
            try
            {
                Assembly[] loadedAsseblies = AppDomain.CurrentDomain.GetAssemblies();
                string[] referencedFiles = loadedAsseblies.Where(a => !a.IsDynamic).Select(a => a.Location).ToArray();

                defaultReferences = new List<MetadataReference>();

                foreach (string referencedFile in referencedFiles)
                {
                    defaultReferences.Add(MetadataReference.CreateFromFile(referencedFile));
                }
            }
            catch(Exception exception)
            {
                throw new Exception("An exception occured when creating the default references used to compile a script", exception);
            }
        }

        public List<MetadataReference> GetDefault()
        {
            return defaultReferences;
        }
    }
}
