using System.Net.Http.Json;
using FluentResults;
using Klayman.Domain;

namespace Klayman.ConsoleApp;

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
        => PostAsync<KeyboardLayout, KeyboardLayoutId>("layouts", layoutId);

    public Task<Result<KeyboardLayout>> RemoveLayoutAsync(KeyboardLayoutId layoutId)
        => DeleteAsync<KeyboardLayout>($"layouts/{layoutId}");
    
    
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
    
    private async Task<Result<TResponse>> PostAsync<TResponse, TRequest>(string route, TRequest request)
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
}