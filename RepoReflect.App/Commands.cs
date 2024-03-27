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
    [Command("reflect-commits")]
    public Task ReflectCommitsToExistingRepo(string privateKey, string projectId,string author, string repoPath, [Option] string customMessage)
    {
        var commits = await _gitLabService.GetGitLabCommits(privateKey, projectId, author);
        
        //TODO: to contributions
        
        return _reflectService.ReflectContributionsToExistingRepo(commits, repoPath);
    }
    
    //Author can be either author email or name
    [Command("reflect-events")]
    public Task ReflectEventsToExistingRepo(string privateKey, string projectId,string author, string repoPath)
    {
        var events = await _gitLabService.GetGitLabEvents(privateKey, projectId, author);
        
        //TODO: map to contributions
        
        return _reflectService.ReflectContributionsToExistingRepo(events, repoPath);
    }

    [Command("create")]
    public Task CreateRepo([Option("n")] string name, [Option("rp")] string repoPath, bool? isPrivate)
    {
        return _repositoryService.CreateGitRepo(name, repoPath, isPrivate);
    }
}