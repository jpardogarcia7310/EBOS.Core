using System.Runtime.Caching;

namespace EBOS.Core.Test;

public class SingletonObjectCacheTests
{
    #region Constructor behavior
    [Fact]
    public void Cache_IsAccessibleAndSameInstance()
    {
        var c1 = SingletonObjectCache.Cache;
        var c2 = MemoryCache.Default;
        Assert.Same(c1, c2);
    }

    [Fact]
    public void Cache_CanStoreAndRetrieveValue()
    {
        var cache = SingletonObjectCache.Cache;
        var key = "test-key";
        cache.Set(key, "value", new CacheItemPolicy { AbsoluteExpiration = System.DateTimeOffset.UtcNow.AddMinutes(1) });
        var value = cache.Get(key);
        Assert.Equal("value", value);
        cache.Remove(key);
    }
    #endregion
}
