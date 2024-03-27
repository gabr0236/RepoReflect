using System.Text.Json.Serialization;

namespace Console.App.Types;

public record GitLabCommit(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt);

public record GitlabEvent(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("project_id")]
    long ProjectId,
    [property: JsonPropertyName("action_name")]
    string ActionName,
    [property: JsonPropertyName("target_id")]
    long? TargetId,
    [property: JsonPropertyName("target_iid")]
    long? TargetIid,
    [property: JsonPropertyName("target_type")]
    string? TargetType,
    [property: JsonPropertyName("author_id")]
    long AuthorId,
    [property: JsonPropertyName("target_title")]
    string TargetTitle,
    [property: JsonPropertyName("created_at")]
    DateTime CreatedAt,
    [property: JsonPropertyName("author")] AuthorInfo Author,
    [property: JsonPropertyName("push_data")]
    PushDataInfo? PushData,
    [property: JsonPropertyName("author_username")]
    string AuthorUsername
);

public record AuthorInfo(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("username")]
    string Username,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("locked")] bool Locked,
    [property: JsonPropertyName("avatar_url")]
    string AvatarUrl,
    [property: JsonPropertyName("web_url")]
    string WebUrl
);

public record PushDataInfo(
    [property: JsonPropertyName("commit_count")]
    int CommitCount,
    [property: JsonPropertyName("action")] string Action,
    [property: JsonPropertyName("ref_type")]
    string RefType,
    [property: JsonPropertyName("commit_from")]
    string CommitFrom,
    [property: JsonPropertyName("commit_to")]
    string CommitTo,
    [property: JsonPropertyName("ref")] string Ref,
    [property: JsonPropertyName("commit_title")]
    string CommitTitle,
    [property: JsonPropertyName("ref_count")]
    int? RefCount
);