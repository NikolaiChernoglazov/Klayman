using Klayman.Domain;

namespace Klayman.Application.KeyboardLayoutSetManagement;

public interface IKeyboardLayoutSetCache
{
    bool Contains(string name);
    
    KeyboardLayoutSet? Get(string name);

    List<KeyboardLayoutSet> GetAll();
    
    void Add(KeyboardLayoutSet layoutSet);

    void Remove(string name);
}