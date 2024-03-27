using System.Text;
using CliWrap;

namespace Console.App.Services;

public class ReflectService
{
    private readonly ILogger<ReflectService> _logger;
    private readonly GitLabService _gitLabService;
    public ReflectService(ILogger<ReflectService> logger, GitLabService gitLabService)
    {
        _logger = logger;
        _gitLabService = gitLabService;
    }

    public async Task ReflectCommitsToExistingRepo(
        string privateKey,
        string projectId,
        string author,
        string pathToRepo,
        string customCommitMessage = "Committed to Master")
    {
        var commits = await _gitLabService.GetGitLabCommitHistory(privateKey, projectId, author);

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

        var events = await _gitLabService.GetGitLabEventHistory(privateKey, projectId, author);

        System.Console.WriteLine("Reflecting Events...");
        
        for (var i = 0; i < events.Count; i++)
        {
            await Cli.Wrap("git")
                .WithArguments($"commit --allow-empty --date \"{events[i].CreatedAt}\" -m \"{Helpers.GetEventName(events[i], _logger)}\" -m \"{events[i].Id}\"") 
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