using System.Collections.Concurrent;
using Klayman.Application.KeyboardLayoutSetManagement;
using Klayman.Domain;

namespace Klayman.Infrastructure.KeyboardLayoutSetManagement;

public class KeyboardLayoutSetCache : IKeyboardLayoutSetCache
{
    private readonly ConcurrentDictionary<string, KeyboardLayoutSet> _cache = new();

    public bool Contains(string name)
    {
        return _cache.ContainsKey(name);
    }

    public KeyboardLayoutSet? Get(string name)
    {
        return _cache.GetValueOrDefault(name);
    }

    public List<KeyboardLayoutSet> GetAll()
    {
        return _cache.Values.ToList();
    }

    public void Add(KeyboardLayoutSet layoutSet)
    {
        _cache.TryAdd(layoutSet.Name, layoutSet);
    }

    public void Remove(string name)
    {
        _cache.Remove(name, out _);
    }
}