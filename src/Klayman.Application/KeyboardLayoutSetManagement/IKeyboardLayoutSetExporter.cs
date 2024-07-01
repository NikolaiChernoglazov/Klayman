using Klayman.Domain.Results;

namespace Klayman.Application.KeyboardLayoutSetManagement;

public interface IKeyboardLayoutSetExporter
{
    Result ExportLayoutSetCacheToJson();

    Result ImportLayoutSetCacheFromJson();
}