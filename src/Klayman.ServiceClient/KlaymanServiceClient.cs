using System.Net.Http.Json;
using FluentResults;
using Klayman.Domain;

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

    
    private async Task<Result<TResponse>> GetAsync<TResponse>(string route)
    {
        try
        {
            var response = await _httpClient.GetAsync(route);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail(response.ToString());
            }

            var content = await response.Content.ReadFromJsonAsync<TResponse>();
            return Result.Ok(content!);
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }
    }
    
    private async Task<Result<TResponse>> PostAsync<TRequest, TResponse>(string route, TRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(route,
                request);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail(response.ToString());
            }

            var content = await response.Content.ReadFromJsonAsync<TResponse>();
            return Result.Ok(content!);
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }
    }

    private async Task<Result<TResponse>> DeleteAsync<TResponse>(string route)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(route);
            if (!response.IsSuccessStatusCode)
            {
                return Result.Fail(response.ToString());
            }

            var content = await response.Content.ReadFromJsonAsync<TResponse>();
            return Result.Ok(content!);
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }
    }

    private async Task<Result> DeleteAsync(string route)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(route);
            return !response.IsSuccessStatusCode
                ? Result.Fail(response.ToString())
                : Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }
    }

    private async Task<Result> OptionsAsync(string route)
    {
        try
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Options,route);
            var response = await _httpClient.SendAsync(requestMessage);
            return !response.IsSuccessStatusCode
                ? Result.Fail(response.ToString())
                : Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }
    }
}