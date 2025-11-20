namespace EBOS.Core.Extensions;

public static class IListExtension
{
    public static bool IsNullOrEmpty<T>(this IList<T>? collection)
    {
        return collection == null || collection.Count == 0;
    }
}
