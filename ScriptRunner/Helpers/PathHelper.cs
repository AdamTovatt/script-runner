namespace ScriptRunner.Helpers
{
    public static class PathHelper
    {
        public static string GetPathFromRelativePath(string relativePath)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        }
    }
}
