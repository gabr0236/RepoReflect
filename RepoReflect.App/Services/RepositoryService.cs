using System.Text;
using CliWrap;

namespace Console.App.Services;

public class RepositoryService
{
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
}