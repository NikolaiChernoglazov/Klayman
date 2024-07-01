using System.Net.Http.Json;
using Klayman.Domain;
using Klayman.Domain.Results;

namespace Klayman.ServiceClient;

public class KlaymanServiceClient
{
    private readonly HttpClient  _httpClient = new()
    {
        BaseAddress = new Uri("http://localhost:5123/")
    };
    
    public Task<Result<KeyboardLayout>> GetCurrentLayoutAsync()
        => GetAsync<KeyboardLayout>("layouts/current");
    
    public Task<Result<List<KeyboardLayout>>> GetCurrentLayoutsAsync()
        => GetAsync<List<KeyboardLayout>>("layouts/");

    public Task<Result<List<KeyboardLayout>>> GetAllAvailableLayoutsAsync()
        => GetAsync<List<KeyboardLayout>>("layouts/all");
    
    public Task<Result<List<KeyboardLayout>>> GetAvailableLayoutsByQueryAsync(string query)
        => GetAsync<List<KeyboardLayout>>($"layouts/all?query={query}");

    public Task<Result<KeyboardLayout>> AddLayoutAsync(KeyboardLayoutId layoutId)
        => PostAsync<KeyboardLayoutId, KeyboardLayout>("layouts", layoutId);

    public Task<Result<KeyboardLayout>> RemoveLayoutAsync(KeyboardLayoutId layoutId)
        => DeleteAsync<KeyboardLayout>($"layouts/{layoutId}");
    
    public Task<Result<List<KeyboardLayoutSet>>> GetLayoutSetsAsync()
        => GetAsync<List<KeyboardLayoutSet>>("layoutSets");
    
    public Task<Result<KeyboardLayoutSet>> AddLayoutSetAsync(AddKeyboardLayoutSetRequest request)
        => PostAsync<AddKeyboardLayoutSetRequest, KeyboardLayoutSet>("layoutSets", request);

    public Task<Result> RemoveLayoutSetAsync(string name)
        => DeleteAsync($"layoutSets/{name}");
    
    public Task<Result> ApplyLayoutSetAsync(string name)
        => OptionsAsync($"layoutSets/{name}/apply");
    

    private async Task<Result> SendAsync(HttpMethod httpMethod, string route)
    {
        try
        {
            var requestMessage = new HttpRequestMessage(
                httpMethod, route);
            var response = await _httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
                return Result.Ok();
            
            var errorContent = await response.Content.ReadFromJsonAsync<
                ErrorResponse>();
            return Result.Fail(errorContent!.Error);

        }
        catch (Exception e)
        {
            return Result.Fail(e.Message, e);
        }
    }
    
    private async Task<Result<TResponse>> SendAsync<TResponse>(
        HttpMethod httpMethod, string route)
    {
        try
        {
            var requestMessage = new HttpRequestMessage(
                httpMethod, route);
            var response = await _httpClient.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadFromJsonAsync<
                    ErrorResponse>();
                return Result.Fail(errorContent!.Error);
            }
            
            var content = await response.Content.ReadFromJsonAsync<TResponse>();
            return Result.Ok(content!);
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message, e);
        }
    }
    
    private Task<Result<TResponse>> GetAsync<TResponse>(string route)
        => SendAsync<TResponse>(HttpMethod.Get, route);
    
    private Task<Result<TResponse>> DeleteAsync<TResponse>(string route)
        => SendAsync<TResponse>(HttpMethod.Delete, route);
    
    private Task<Result> DeleteAsync(string route)
        => SendAsync(HttpMethod.Delete, route);
    
    private Task<Result> OptionsAsync(string route)
        => SendAsync(HttpMethod.Options, route);
    
    private async Task<Result<TResponse>> PostAsync<TRequest, TResponse>(string route, TRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(route,
                request);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadFromJsonAsync<
                    ErrorResponse>();
                return Result.Fail(errorContent!.Error);
            }

            var content = await response.Content.ReadFromJsonAsync<TResponse>();
            return Result.Ok(content!);
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message, e);
        }
    }
}