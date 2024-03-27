using CliWrap;
using Console.App.Types;

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
    
    private async Task ReflectContribution(Contribution contribution,string pathToRepo)
    {
        await Cli.Wrap("git")
            .WithArguments($"commit --allow-empty --date \"{contribution.CreatedAt}\" -m \"{contribution.Message}\" -m \"{contribution.Id}\"") 
            .WithWorkingDirectory(pathToRepo)
            .WithValidation(CommandResultValidation.None) // For some reason "git commit --allow-empty returns a non-zero exit code"
            .ExecuteAsync();
    }

    public async Task ReflectContributionsToExistingRepo(
        List<Contribution> contributions,
        string pathToRepo)
    {
        System.Console.WriteLine("Reflecting...");

        for (var i = 0; i < contributions.Count; i++)
        {
            await ReflectContribution(contributions[i], pathToRepo);

            var progress = i * 100 / contributions.Count;
            System.Console.Write("\r{0}% ", progress);
        }
        System.Console.WriteLine("\r100%");
        
        System.Console.WriteLine($"Successfully added {contributions.Count} Contributions to {pathToRepo}");
    }
}