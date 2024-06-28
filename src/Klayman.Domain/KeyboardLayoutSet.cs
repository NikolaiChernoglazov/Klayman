using System.Text;

namespace Klayman.Domain;

public record KeyboardLayoutSet(
    string Name,
    List<KeyboardLayout> Layouts)
{
    public List<KeyboardLayoutId> GetLayoutIds()
        => Layouts.Select(l => l.Id).ToList();
    
    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Name: {Name}");
        Layouts.ForEach(l => builder.AppendLine(l.ToString()));
        return builder.ToString();
    }
}