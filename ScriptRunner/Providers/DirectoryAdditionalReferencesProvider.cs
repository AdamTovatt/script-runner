namespace ScriptRunner.Providers
{
    public class DirectoryAdditionalReferencesProvider : IAdditionalReferencesProvider
    {
        private const string additionalReferencesFileName = "additional-references.txt";

        public List<string>? GetAdditionalReferences()
        {
            if (File.Exists(additionalReferencesFileName))
                return File.ReadAllLines(additionalReferencesFileName).ToList();

            return null;
        }

        public void PersistReference(string reference)
        {
            List<string> existingReferences = GetAdditionalReferences() ?? new List<string>();

            if (!existingReferences.Contains(reference))
                existingReferences.Add(reference);

            File.WriteAllLines(additionalReferencesFileName, existingReferences);
        }
    }
}
