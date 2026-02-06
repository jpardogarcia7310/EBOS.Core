using EBOS.Core.Primitives.Interfaces;
using System.Collections.ObjectModel;

namespace EBOS.Core;

/// <summary>
/// Helper KeyedCollection that uses Code as the key.
/// </summary>
public sealed class KeyedErrors : KeyedCollection<int, IErrorResult>
{
    public KeyedErrors() : base() { }
    public KeyedErrors(IEqualityComparer<int>? comparer) : base(comparer) { }

    protected override int GetKeyForItem(IErrorResult item)
    {
        if (item is null) 
            throw new ArgumentNullException(nameof(item));
        return item.Code;
    }

    public bool ContainsKey(int key) => Dictionary is not null ? Dictionary.ContainsKey(key) : Items.Any(i => GetKeyForItem(i) == key);

    public bool TryAdd(IErrorResult item)
    {
        if (item is null) 
            throw new ArgumentNullException(nameof(item));
        var key = GetKeyForItem(item);
        if (ContainsKey(key)) return false;
        Add(item);
        return true;
    }

    public void AddOrUpdate(IErrorResult item)
    {
        if (item is null) 
            throw new ArgumentNullException(nameof(item));
        var key = GetKeyForItem(item);
        if (ContainsKey(key))
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (GetKeyForItem(Items[i]) == key)
                {
                    SetItem(i, item);
                    return;
                }
            }
        }
        else
        {
            Add(item);
        }
    }
}
