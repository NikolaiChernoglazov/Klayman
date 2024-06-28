using FluentResults;
using Klayman.Domain;

namespace Klayman.Application.KeyboardLayoutSetManagement;

public interface IKeyboardLayoutSetManager
{
    List<KeyboardLayoutSet> GetLayoutSets();

    Result<KeyboardLayoutSet> AddLayoutSet(
        string name, List<KeyboardLayoutId> layoutIds);

    Result RemoveLayoutSet(string name);
    
    Result ApplyLayoutSet(string name);
}