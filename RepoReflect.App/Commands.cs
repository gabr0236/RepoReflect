using Cocona;
using Console.App.Services;

namespace Console.App;

public class Commands
{
    private readonly ReflectService _reflectService;
    private readonly RepositoryService _repositoryService;

    public Commands(ReflectService reflectService, RepositoryService repositoryService)
    {
        _reflectService = reflectService;
        _repositoryService = repositoryService;
    }

    //Author can be either author email or name
    [Command("reflect-commits")]
    public Task ReflectCommitsToExistingRepo(string privateKey, string projectId,string author, string repoPath, [Option] string customMessage)
    {
        return _reflectService.ReflectCommitsToExistingRepo(privateKey, projectId, author, repoPath, customMessage);
    }
    
    //Author can be either author email or name
    [Command("reflect-events")]
    public Task ReflectEventsToExistingRepo(string privateKey, string projectId,string author, string repoPath)
    {
        return _reflectService.ReflectEventsToExistingRepo(privateKey, projectId, author, repoPath);
    }

    [Command("create")]
    public Task CreateRepo([Option("n")] string name, [Option("rp")] string repoPath, bool? isPrivate)
    {
        return _repositoryService.CreateGitRepo(name, repoPath, isPrivate);
    }
}