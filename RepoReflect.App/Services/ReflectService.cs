using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CliWrap;
using Console.App.Types;

namespace Console.App.Services;

public class ReflectService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ReflectService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<GitLabCommit>> GetGitLabCommitHistory(string privateKey, string projectId, string author)
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
                    $"https://gitlab.com/api/v4/projects/{projectId}/repository/commits?page={page}&per_page=99&sort=asc&author={author}");

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

        return commits;
    }
    
    public async Task<List<GitlabEvent>> GetGitLabEventHistory(string privateKey, string projectId, string author)
    {
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", privateKey);

        var events = new List<GitlabEvent>();

        var page = 1;
        bool hasMorePages = true;

        while (hasMorePages)
        {
            var response =
                await client.GetAsync(
                    $"https://gitlab.com/api/v4/events?page={page}&per_page=99&sort=asc&scope={projectId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error while requesting history");
            }

            var content = await response.Content.ReadAsStringAsync();
            var newEvents = JsonSerializer.Deserialize<List<GitlabEvent>>(content);
            events.AddRange(newEvents!);

            page++;
            if (newEvents!.Count == 0) hasMorePages = false;
        }

        System.Console.WriteLine($"{author} has this many events: {events.Count}");

        return events;
    }

    public async Task ReflectCommitsToExistingRepo(string privateKey, string projectId, string author, string pathToRepo, string customCommitMessage = "Committed to Master")
    {
        var commits = await GetGitLabCommitHistory(privateKey, projectId, author);

        System.Console.WriteLine("Reflecting Commits...");

        for (var i = 0; i < commits.Count; i++)
        {
            await Cli.Wrap("git")
                .WithArguments($"commit --allow-empty --date \"{commits[i].CreatedAt}\" -m \"{customCommitMessage}\" -m \"{commits[i].Id}\"") 
                .WithWorkingDirectory(pathToRepo)
                .WithValidation(CommandResultValidation.None) // For some reason "git commit --allow-empty returns a non-zero exit code"
                .ExecuteAsync();

            var progress = i * 100 / commits.Count;
            System.Console.Write("\r{0}% ", progress);
        }
        System.Console.WriteLine("\r100%");
        
        System.Console.WriteLine($"Successfully added {commits.Count} Commits to {pathToRepo}");
    }
    
    public async Task ReflectEventsToExistingRepo(string privateKey, string projectId, string author, string pathToRepo)
    {

        var events = await GetGitLabEventHistory(privateKey, projectId, author);

        System.Console.WriteLine("Reflecting Events...");
        
        string message = "";
        for (var i = 0; i < events.Count; i++)
        {
            message = events[i].ActionName + " " + events[i].TargetType ?? "unknown";
            
            //TODO: also edit the time of this commit
            await Cli.Wrap("git")
                .WithArguments($"commit --allow-empty --date \"{events[i].CreatedAt}\" -m \"{message}\" -m \"{events[i].Id}\"") 
                .WithWorkingDirectory(pathToRepo)
                .WithValidation(CommandResultValidation.None) // For some reason "git commit --allow-empty returns a non-zero exit code"
                .ExecuteAsync();

            var progress = i * 100 / events.Count;
            System.Console.Write("\r{0}% ", progress);
        }
        System.Console.WriteLine("\r100%");
        
        System.Console.WriteLine($"Successfully added {events.Count} Events to {pathToRepo}");
    }

    public async Task CreateGitRepo(string repoName, string repoDirPath, bool? isPrivate = false)
    {
        var sb = new StringBuilder();
        await Cli.Wrap("/bin/bash")
            .WithArguments($"-c \"cd {repoDirPath} && mkdir {repoName} && cd {repoName} && touch README.md && pwd\"")
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb)).ExecuteAsync();

        var repoPath = repoDirPath + "/" + repoName;
        
         await Cli.Wrap("/bin/bash")
            .WithArguments($"-c \"git init && git add . && git commit -m \"Initial Commit\"\"")
            .WithWorkingDirectory(repoPath)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb)).ExecuteAsync();
         
         var repoHttpsUrl = string.Empty;
         await Cli.Wrap("gh")
             .WithArguments($"repo create {repoName} {(isPrivate!.Value ? "--private" : "")}")
             .WithStandardOutputPipe(PipeTarget.ToDelegate((res) => repoHttpsUrl = res ))
             .ExecuteAsync();
         
         var split = repoHttpsUrl.Split("/");
         var usernameAndRepoName = split[4] + "/" + split[5]; //https://github.com/user/repo
         var sshUrl = $"git@github.com:{usernameAndRepoName}.git";
         
         await Cli.Wrap("git")
             .WithArguments($"remote add origin {sshUrl}")
             .WithWorkingDirectory(repoPath)
             .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb)).ExecuteAsync();
         
         await Cli.Wrap("git")
             .WithArguments($"push -u origin master")
             .WithWorkingDirectory(repoPath)
             .WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb)).ExecuteAsync();
         
        System.Console.WriteLine(sb);
    }

    public async Task UpdateExistingRepo(string repoRelativePath)
    {
        
    }
}