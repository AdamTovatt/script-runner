using Microsoft.CodeAnalysis;
using System.Reflection;

namespace ScriptRunner.Providers
{
    /// <summary>
    /// Used to provide references for the compilation of code
    /// </summary>
    public class ReferenceProvider
    {
        /// <summary>
        /// Will get an instance of the ReferenceProvider
        /// </summary>
        public static ReferenceProvider Instance { get { if (_instance == null) _instance = new ReferenceProvider(); return _instance; } }
        private static ReferenceProvider? _instance;

        public IAdditionalReferencesProvider? AdditionalReferencesProvider { get; set; }

        private List<MetadataReference> references;

        /// <summary>
        /// Constructor for creating a ReferenceProvider, should not be used manually, use the ReferenceProvider.Instance to get an instance instead
        /// </summary>
        /// <exception cref="Exception"></exception>
        public ReferenceProvider()
        {
            try
            {
                Assembly[] loadedAsseblies = AppDomain.CurrentDomain.GetAssemblies();
                string[] referencedFiles = loadedAsseblies.Where(a => !a.IsDynamic).Select(a => a.Location).ToArray();

                references = new List<MetadataReference>();

                foreach (string referencedFile in referencedFiles)
                {
                    references.Add(MetadataReference.CreateFromFile(referencedFile));
                }
            }
            catch (Exception exception)
            {
                throw new Exception("An exception occured when creating the default references used to compile a script", exception);
            }
        }

        /// <summary>
        /// Will get the references
        /// </summary>
        /// <returns></returns>
        public List<MetadataReference> GetReferences()
        {
            return references;
        }

        /// <summary>
        /// Will add a reference to the list of references
        /// </summary>
        /// <param name="reference"></param>
        public void AddReference(string reference)
        {
            Assembly assembly = Assembly.Load(reference);

            references.Add(MetadataReference.CreateFromFile(assembly.Location));
        }

        /// <summary>
        /// Will persist a references to the list of references to be loaded automatically in the future
        /// </summary>
        /// <param name="reference"></param>
        public void PersistReference(string reference)
        {
            if (AdditionalReferencesProvider != null)
            {
                Assembly assembly = Assembly.Load(reference);

                AdditionalReferencesProvider.PersistReference(assembly.Location);
            }
        }

        /// <summary>
        /// Will load additional references
        /// </summary>
        public void LoadAdditionalReferences()
        {
            if (AdditionalReferencesProvider != null)
            {
                List<string>? additionalReferences = AdditionalReferencesProvider.GetAdditionalReferences();

                if (additionalReferences == null) return;

                foreach (string reference in additionalReferences)
                {
                    references.Add(MetadataReference.CreateFromFile(reference));
                }
            }
        }
    }
}
