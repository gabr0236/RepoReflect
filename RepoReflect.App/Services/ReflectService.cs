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

    public async Task UpdateExistingRepo(string repoRelativePath)
    {
        
    }
}