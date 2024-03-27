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

    public async Task<(List<GitLabCommit> commits, List<GitlabEvent> events)> GetAllGitLabContributions(string privateKey, string projectId,
        string author)
    {
        var getCommitsTask = GetGitLabCommits(privateKey, projectId, author);
        var getEventsTask =  GetGitLabEvents(privateKey, projectId, author);

        await Task.WhenAll(getCommitsTask, getEventsTask);

        var commits = await getCommitsTask;
        var events = await getEventsTask;
        
        return (commits, events);
    }

    public async Task<List<GitLabCommit>> GetGitLabCommits(string privateKey, string projectId, string author)
    {
        var requestUri =
            $"https://gitlab.com/api/v4/projects/{projectId}/repository/commits?author={author}";

        var commits = await GetGitLabContributions<GitLabCommit>(privateKey, requestUri);
        
        System.Console.WriteLine($"{author} has this many commits: {commits.Count}");

        return commits;
    }
    
    public async Task<List<GitlabEvent>> GetGitLabEvents(string privateKey, string projectId, string author)
    {
        var requestUri = $"https://gitlab.com/api/v4/events?scope={projectId}";

        var events = await GetGitLabContributions<GitlabEvent>(privateKey, requestUri);
        
        System.Console.WriteLine($"{author} has this many events: {events.Count}");

        return events;
    }

    //TODO: request uri must already have a previous url param as we are applying & below
    private async Task<List<T>> GetGitLabContributions<T>(string privateKey, string requestUri)
    {
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", privateKey);

        var contributions = new List<T>();

        var page = 1;
        bool hasMorePages = true;

        while (hasMorePages)
        {
            var response =
                await client.GetAsync(
                    $"{requestUri}&page={page}&per_page=99&sort=asc");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error while requesting history");
            }

            var content = await response.Content.ReadAsStringAsync();
            var newContributions = JsonSerializer.Deserialize<List<T>>(content);
            contributions.AddRange(newContributions!);

            page++;
            if (newContributions!.Count == 0) hasMorePages = false;
        }
        
        return contributions;
    }
}