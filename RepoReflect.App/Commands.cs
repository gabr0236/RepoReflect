using Cocona;
using Console.App.Services;

namespace Console.App;

public class Commands
{
    private readonly ReflectService _reflectService;

    public Commands(ReflectService reflectService)
    {
        _reflectService = reflectService;
    }

    //Author can be either author email or name
    [Command("reflect-commits")]
    public Task ReflectCommitsToExistingRepo(string privateKey, string projectId,string author, string repoPath, [Option] string customMessage)
    {
        return _reflectService.ReflectCommitsToExistingRepo(privateKey, projectId, author, repoPath, customMessage);
    }

    [Command("create")]
    public Task test([Option("n")] string name, [Option("rp")] string repoPath)
    {
        return _reflectService.CreateGitRepo(name, repoPath);
    }
}