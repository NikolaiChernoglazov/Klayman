namespace Klayman.Domain;

public record AddKeyboardLayoutSetRequest(
    string Name,
    List<KeyboardLayoutId> LayoutIds);
