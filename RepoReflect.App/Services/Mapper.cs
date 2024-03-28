using Console.App.Types;

namespace Console.App.Services;

public class Mapper
{
    public static Contribution FromGitLabCommit(GitLabCommit gitLabCommit)
    {
        return new Contribution(gitLabCommit.Id, gitLabCommit.CreatedAt, "Committed to Master"); //TODO allow for setting/passing commit message?
    }
    
    public static Contribution FromGitLabEvent(GitlabEvent gitlabEvent)
    {
        //TODO: avoid this toString by initially casting long to string at json parsing? leave note in GitLabEvent on id Prop about changing type
        return new Contribution(gitlabEvent.Id.ToString(), gitlabEvent.CreatedAt, Helpers.GetEventName(gitlabEvent));
    }

    public static List<Contribution> FromGitLabCommits(List<GitLabCommit> gitLabCommits) => gitLabCommits.Select(FromGitLabCommit).ToList();

    public static List<Contribution> FromGitLabEvents(List<GitlabEvent> gitlabEvents) =>
        gitlabEvents.Select(FromGitLabEvent).ToList();

    public static List<Contribution> FromAll(List<GitLabCommit> commits, List<GitlabEvent> events)
    {
        var contributions = FromGitLabCommits(commits);
        contributions.AddRange(FromGitLabEvents(events));
        
        contributions.Sort((a,b) => a.CreatedAt.CompareTo(b.CreatedAt));

        return contributions;
    }
}