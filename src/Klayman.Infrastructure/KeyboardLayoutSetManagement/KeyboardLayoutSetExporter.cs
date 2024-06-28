using System.Text.Json;
using FluentResults;
using Klayman.Application.KeyboardLayoutSetManagement;
using Klayman.Domain;
using Klayman.Domain.JsonConverters;

namespace Klayman.Infrastructure.KeyboardLayoutSetManagement;

public class KeyboardLayoutSetExporter(
    IKeyboardLayoutSetCache layoutSetCache) : IKeyboardLayoutSetExporter
{
    private const string _fileName = "keyboardLayoutSets.json";
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new KeyboardLayoutIdJsonConverter() }
    };
    
    public Result ExportLayoutSetCacheToJson()
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(
                layoutSetCache.GetAll(), _serializerOptions);
            File.WriteAllText(_fileName, jsonString);
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }
    }

    public Result ImportLayoutSetCacheFromJson()
    {
        try
        {
            if (!File.Exists(_fileName))
            {
                return Result.Ok();
            }
            
            var jsonString = File.ReadAllText(_fileName);
            var layoutSets = JsonSerializer.Deserialize<List<KeyboardLayoutSet>>(
                jsonString, _serializerOptions)!;
            layoutSets.ForEach(layoutSetCache.Add);
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError(e));
        }
    }
}