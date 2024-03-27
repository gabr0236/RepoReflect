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

        System.Console.WriteLine("Reflecting...");

        for (var i = 0; i < commits.Count; i++)
        {
            await ReflectCommit(commits[i].CreatedAt, customCommitMessage, commits[i].Id, pathToRepo);

            var progress = i * 100 / commits.Count;
            System.Console.Write("\r{0}% ", progress);
        }
        System.Console.WriteLine("\r100%");
        
        System.Console.WriteLine($"Successfully added {commits.Count} Contributions to {pathToRepo}");
    }
    
    public async Task ReflectEventsToExistingRepo(string privateKey, string projectId, string author, string pathToRepo)
    {

        var events = await _gitLabService.GetGitLabEventHistory(privateKey, projectId, author);

        System.Console.WriteLine("Reflecting...");
        
        for (var i = 0; i < events.Count; i++)
        {
            await ReflectEvent(events[i].CreatedAt, Helpers.GetEventName(events[i], _logger), events[i].Id, pathToRepo);
            
            var progress = i * 100 / events.Count;
            System.Console.Write("\r{0}% ", progress);
        }
        System.Console.WriteLine("\r100%");
        
        System.Console.WriteLine($"Successfully added {events.Count} Contributions to {pathToRepo}");
    }
    
    private async Task ReflectCommit(DateTime createdAt, string message, string originalSha, string pathToRepo)
    {
        await Cli.Wrap("git")
            .WithArguments($"commit --allow-empty --date \"{createdAt}\" -m \"{message}\" -m \"{originalSha}\"") 
            .WithWorkingDirectory(pathToRepo)
            .WithValidation(CommandResultValidation.None) // For some reason "git commit --allow-empty returns a non-zero exit code"
            .ExecuteAsync();
    }
    
    private async Task ReflectEvent(DateTime createdAt, string message, long id, string pathToRepo)
    {
        await Cli.Wrap("git")
            .WithArguments($"commit --allow-empty --date \"{createdAt}\" -m \"{message}\" -m \"{id}\"") 
            .WithWorkingDirectory(pathToRepo)
            .WithValidation(CommandResultValidation.None) // For some reason "git commit --allow-empty returns a non-zero exit code"
            .ExecuteAsync();
    }
}