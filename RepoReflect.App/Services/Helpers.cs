using Console.App.Types;

namespace Console.App.Services;

public static class Helpers
{
    public static string GetEventName(GitlabEvent gitlabEvent, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(gitlabEvent.ActionName))
        {
            logger.LogDebug("Unknown event found with event id: {GitlabEventId}", gitlabEvent.Id);
            return "Unknown Event";
        }

        var message = UppercaseFirst(gitlabEvent.ActionName) + " ";
        
        if (string.IsNullOrWhiteSpace(gitlabEvent.TargetType))
        {
            // GitLab events for Branch activity eg. push, create, delete doesnt come with .TargetType
            // Although such events come with .PushData attribute from where we can infer the type
            if (gitlabEvent.PushData is not null)
            {
                message += UppercaseFirst(gitlabEvent.PushData.RefType);
            }
            else
            {
                // More Event types may exist without .TargetType. For now we leave these as Unknown
                message += "Unknown";
            }
        }
        else
        {
            message += gitlabEvent.TargetType;
        }

        return message;
    }
    
    public static string UppercaseFirst(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        var a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }
}