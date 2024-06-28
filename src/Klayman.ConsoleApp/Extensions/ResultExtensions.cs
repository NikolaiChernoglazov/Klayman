using FluentResults;

namespace Klayman.ConsoleApp.Extensions;

public static class ResultExtensions
{
    public static string GetCombinedErrorMessage(this Result result)
    {
        return string.Join(", ", result.Errors.Select(e => e.Message));
    }
    
    public static string GetCombinedErrorMessage<T>(this Result<T> result)
    {
        return string.Join(", ", result.Errors.Select(e => e.Message));
    }
}