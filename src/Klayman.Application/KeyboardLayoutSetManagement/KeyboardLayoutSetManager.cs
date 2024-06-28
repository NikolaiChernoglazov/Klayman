using FluentResults;
using Klayman.Application.KeyboardLayoutManagement;
using Klayman.Domain;

namespace Klayman.Application.KeyboardLayoutSetManagement;

public class KeyboardLayoutSetManager(
    IKeyboardLayoutSetCache layoutSetCache,
    IKeyboardLayoutFactory keyboardLayoutFactory,
    IKeyboardLayoutManager keyboardLayoutManager) : IKeyboardLayoutSetManager
{
    public List<KeyboardLayoutSet> GetLayoutSets()
    {
        return layoutSetCache.GetAll();
    }

    public Result<KeyboardLayoutSet> AddLayoutSet(string name, List<KeyboardLayoutId> layoutIds)
    {
        if (layoutSetCache.Contains(name))
        {
            return Result.Fail($"Keyboard layout set {name} already exists.");
        }

        var canApplyLayoutsResult = CanApplyLayouts(layoutIds);
        if (canApplyLayoutsResult.IsFailed)
        {
            return canApplyLayoutsResult;
        }

        var layoutSet = new KeyboardLayoutSet(
            name, layoutIds.Select(keyboardLayoutFactory.CreateFromKeyboardLayoutId).ToList());
        layoutSetCache.Add(layoutSet);
        return Result.Ok(layoutSet);
    }

    public Result RemoveLayoutSet(string name)
    {
        if (!layoutSetCache.Contains(name))
        {
            return Result.Fail($"Keyboard layout set {name} does not exist.");
        }

        layoutSetCache.Remove(name);
        return Result.Ok();
    }

    public Result ApplyLayoutSet(string name)
    {
        if (!layoutSetCache.Contains(name))
        {
            return Result.Fail($"Keyboard layout set {name} does not exist.");
        }

        var layoutSet = layoutSetCache.Get(name)!;
        var canApplyLayoutsResult = CanApplyLayouts(layoutSet.GetLayoutIds());
        if (canApplyLayoutsResult.IsFailed)
        {
            return canApplyLayoutsResult;
        }

        var currentLayoutsResult = keyboardLayoutManager.GetCurrentLayouts();
        if (currentLayoutsResult.IsFailed)
        {
            return currentLayoutsResult.ToResult();
        }
        
        // Primitive logic for now. Later we can can define intersection with the
        // current list of keyboard layouts and remove/add only required ones
        var removalResult = currentLayoutsResult.Value.Skip(1)
            .Select(l => keyboardLayoutManager.RemoveLayout(l.Id)).Merge();
        if (removalResult.IsFailed)
        {
            return removalResult.ToResult();
        }

        var addResult = layoutSetCache.Get(name)!.Layouts.Select(
            l => keyboardLayoutManager.AddLayout(l.Id)).Merge();

        return addResult.ToResult();
    }

    private Result CanApplyLayouts(List<KeyboardLayoutId> layoutIds)
    {
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var layoutId in layoutIds)
        {
            var canAddLayoutResult = keyboardLayoutManager.CanAddLayout(layoutId);
            if (canAddLayoutResult.IsFailed)
            {
                return canAddLayoutResult;
            }
        }

        return Result.Ok();
    }
}