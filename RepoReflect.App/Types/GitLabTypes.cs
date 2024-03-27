using System.Text.Json.Serialization;

namespace Console.App.Types;

public record GitLabCommit(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("created_at")] DateTime CreatedAt);

public record GitlabEvent(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("action_name")]
    string ActionName,
    [property: JsonPropertyName("target_type")]
    string? TargetType,
    [property: JsonPropertyName("created_at")]
    DateTime CreatedAt,
    [property: JsonPropertyName("push_data")]
    PushDataInfo? PushData
);

public record PushDataInfo(
    [property: JsonPropertyName("ref_type")]
    string RefType
);