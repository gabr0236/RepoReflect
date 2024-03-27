using System.Text.Json;
using Console.App.Types;

namespace Console.App.Services;

public class GitLabService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GitLabService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<GitLabCommit>> GetGitLabCommitHistory(string privateKey, string projectId, string author)
    {
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", privateKey);

        var commits = new List<GitLabCommit>();

        var page = 1;
        bool hasMorePages = true;

        while (hasMorePages)
        {
            var response =
                await client.GetAsync(
                    $"https://gitlab.com/api/v4/projects/{projectId}/repository/commits?page={page}&per_page=99&sort=asc&author={author}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error while requesting history");
            }

            var content = await response.Content.ReadAsStringAsync();
            var newCommits = JsonSerializer.Deserialize<List<GitLabCommit>>(content);
            commits.AddRange(newCommits!);

            page++;
            if (newCommits!.Count == 0) hasMorePages = false;
        }

        System.Console.WriteLine($"{author} has this many commits: {commits.Count}");

        return commits;
    }
    
    public async Task<List<GitlabEvent>> GetGitLabEventHistory(string privateKey, string projectId, string author)
    {
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", privateKey);

        var events = new List<GitlabEvent>();

        var page = 1;
        bool hasMorePages = true;

        while (hasMorePages)
        {
            var response =
                await client.GetAsync(
                    $"https://gitlab.com/api/v4/events?page={page}&per_page=99&sort=asc&scope={projectId}");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error while requesting history");
            }

            var content = await response.Content.ReadAsStringAsync();
            var newEvents = JsonSerializer.Deserialize<List<GitlabEvent>>(content);
            events.AddRange(newEvents!);

            page++;
            if (newEvents!.Count == 0) hasMorePages = false;
        }

        System.Console.WriteLine($"{author} has this many events: {events.Count}");

        return events;
    }
}