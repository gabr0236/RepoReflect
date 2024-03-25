using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CliWrap;

namespace Console.App.Services;

public class ReflectService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ReflectService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task GetHistory(string privateKey, string projectId, string author)
    {
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", privateKey);

        var commits = new List<GitLabCommit>();

        var page = 1;
        bool hasMorePages = true;

        while (hasMorePages)
        {
            var response =
                await client.GetAsync(
                    $"https://gitlab.com/api/v4/projects/{projectId}/repository/commits?page={page}&per_page=99&author={author}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error while requesting history");
            }

            var content = await response.Content.ReadAsStringAsync();
            var newCommits = JsonSerializer.Deserialize<List<GitLabCommit>>(content);
            commits.AddRange(newCommits!);

            page++;
            if (newCommits!.Count == 0) hasMorePages = false;
        }

        System.Console.WriteLine($"{author} has this many commits: {commits.Count}");
        System.Console.WriteLine("Exiting GetHistory");
    }

    public async Task CreateGitRepo(string repoName, string repoRelativeCreatePath)
    {
        var sb = new StringBuilder();
        await Cli.Wrap("/bin/bash")
            .WithArguments($"-c \"cd {repoRelativeCreatePath} && mkdir {repoName} && cd {repoName} && touch README.md && pwd\"")
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb)).ExecuteAsync();

        var repoRelativePath = repoRelativeCreatePath + "/" + repoName;
        
         await Cli.Wrap("/bin/bash")
            .WithArguments($"-c \"git init && git add . && git commit -m \"Initial Commit\"\"")
            .WithWorkingDirectory(repoRelativePath)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb)).ExecuteAsync();
        
        System.Console.WriteLine(sb);
    }

    private record GitLabCommit(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("title")] string Title, //TODO: delete
        [property: JsonPropertyName("created_at")]
        DateTime CreatedAt,
        [property: JsonPropertyName("message")]
        string Message); //TODO: delete
}