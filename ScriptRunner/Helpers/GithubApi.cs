using Octokit;

namespace ScriptRunner.Helpers
{
    public class GithubApi : IGithubApi
    {
        private GitHubClient client;
        private string owner;
        private string repoName;
        private string branch;

        public GithubApi(string githubToken, string productHeaderValue, string owner, string repoName, string branch)
        {
            client = new GitHubClient(new ProductHeaderValue(productHeaderValue));
            client.Credentials = new Credentials(githubToken);
            this.owner = owner;
            this.repoName = repoName;
            this.branch = branch;
        }

        public async Task CreateScript(string name, string code)
        {
            string filePath = $"{name}.cs";
            await client.Repository.Content.CreateFile(owner, repoName, filePath, new CreateFileRequest($"Created {filePath}", code, branch));
        }

        public async Task UpdateScript(string name, string code)
        {
            string filePath = $"{name}.cs";
            try
            {
                IReadOnlyList<RepositoryContent>? fileDetails = await client.Repository.Content.GetAllContentsByRef(owner, repoName, filePath, branch);
                await client.Repository.Content.UpdateFile(owner, repoName, filePath, new UpdateFileRequest($"Updated {filePath}", code, fileDetails.First().Sha));
            }
            catch
            {
                await CreateScript(name, code);
            }
        }
    }
}
