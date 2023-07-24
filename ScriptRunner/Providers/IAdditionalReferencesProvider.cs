namespace ScriptRunner.Providers
{
    public interface IAdditionalReferencesProvider
    {
        public List<string>? GetAdditionalReferences();
        public void PersistReference(string references);
    }
}
