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
}