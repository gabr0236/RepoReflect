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
    [Command("reflect")]
    public Task Reflect(string privateKey, string projectId, string author, string repoPath)
    {
        return _reflectService.ReflectNewRepo(privateKey, projectId, author, repoPath );
    }

    [Command("create")]
    public Task test([Option("n")] string name, [Option("rp")] string repoPath)
    {
        return _reflectService.CreateGitRepo(name, repoPath);
    }
}