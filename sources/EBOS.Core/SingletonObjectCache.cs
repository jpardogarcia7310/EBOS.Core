using System.Runtime.Caching;

namespace EBOS.Core;

public static class SingletonObjectCache
{
    public static readonly ObjectCache Cache = MemoryCache.Default;
}
