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

    [Command("reflect")]
    public Task Reflect(string privateKey, string projectId, string author)
    {
        return _reflectService.GetHistory(privateKey, projectId, author);
    }

    [Command("test")]
    public Task test([Option("n")] string name, [Option("rp")] string relativePath)
    public Task test([Option("n")] string name, [Option("rp")] string repoPath)
    {
        System.Console.WriteLine(name);
        System.Console.WriteLine(repoPath);
        
        return _reflectService.CreateGitRepo(name, repoPath);
    }
}