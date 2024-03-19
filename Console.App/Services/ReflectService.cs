using System.Text.Json;
using System.Text.Json.Serialization;

namespace Console.App.Services;

public class ReflectService
{

    private readonly IHttpClientFactory _httpClientFactory;

    public ReflectService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    /*
     curl --header "PRIVATE-TOKEN: TOKEN" \
       --url "https://gitlab.com/api/v4/projects/PROJECTID/repository/commits"
     */

    public async Task GetHistory(string privateKey, string projectId, string author)
    {
        var client = _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Add("PRIVATE-TOKEN", privateKey);

        var commits = new List<GitLabCommit>();
        
        var page = 1;
        bool hasMorePages = true;
        
        while(hasMorePages)
        {
            var response = await client.GetAsync($"https://gitlab.com/api/v4/projects/{projectId}/repository/commits?page={page}&per_page=99&author={author}");
            
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
        System.Console.WriteLine("Exiting GetHistory");
    }

    private record GitLabCommit(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("title")] string Title, //TODO: delete
        [property: JsonPropertyName("created_at")] DateTime CreatedAt,
        [property: JsonPropertyName("message")] string Message); //TODO: delete
}