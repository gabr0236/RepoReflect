using Cocona;
using Console.App.Services;

namespace Console.App;

public class Commands
{
    private readonly ReflectService _reflectService;
    private readonly RepositoryService _repositoryService;
    private readonly GitLabService _gitLabService;

    public Commands(ReflectService reflectService, RepositoryService repositoryService, GitLabService gitLabService)
    {
        _reflectService = reflectService;
        _repositoryService = repositoryService;
        _gitLabService = gitLabService;
    }

    //Author can be either author email or name
    [Command("reflect-all")]
    public async Task ReflectContributions(string privateKey, string projectId, string author, string repoPath)
    {
        var (commits, events) = await _gitLabService.GetAllGitLabContributions(privateKey, projectId, author);

        var contributions = Mapper.FromAll(commits, events);

        await _reflectService.ReflectContributionsToExistingRepo(contributions, repoPath);
    }

    //Author can be either author email or name
    [Command("reflect-commits")]
    public async Task ReflectCommitsToExistingRepo(string privateKey, string projectId,string author, string repoPath)
    {
        var commits = await _gitLabService.GetGitLabCommits(privateKey, projectId, author);

        var contributions = Mapper.FromGitLabCommits(commits);
        
        await _reflectService.ReflectContributionsToExistingRepo(contributions, repoPath);
    }
    
    //Author can be either author email or name
    [Command("reflect-events")]
    public async Task ReflectEventsToExistingRepo(string privateKey, string projectId,string author, string repoPath)
    {
        var events = await _gitLabService.GetGitLabEvents(privateKey, projectId, author);

        var contributions = Mapper.FromGitLabEvents(events);
        
        await _reflectService.ReflectContributionsToExistingRepo(contributions, repoPath);
    }

    [Command("create")]
    public Task CreateRepo([Option("n")] string name, [Option("rp")] string repoPath, bool? isPrivate)
    {
        return _repositoryService.CreateGitRepo(name, repoPath, isPrivate);
    }

    [Command("test")]
    public async Task test()
    {
        var progress = 0;
        for (var i = 0; i < 50; i++)
        {
            System.Console.WriteLine(progress);
            progress = (i*100) / 50;
            System.Console.Write("\r{0}%   ", progress);
        }
    }
}