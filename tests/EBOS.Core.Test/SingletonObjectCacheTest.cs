using System.Runtime.Caching;

namespace EBOS.Core.Test;

public class SingletonObjectCacheTests
{
    #region Helper derived class to access protected members
    private class TestSingletonObjectCache : SingletonObjectCache
    {
        // Exponemos el campo protegido estático para poder probarlo
        public static ObjectCache Cache => SingletonObjectCache.Cache;
    }
    #endregion

    #region Constructor behavior
    [Fact]
    public void CanInstantiate_DerivedClass_ButNotBaseClass()
    {
        // Podemos crear una instancia de la clase derivada:
        var instance = new TestSingletonObjectCache();

        Assert.NotNull(instance);

        // No podemos comprobar en tests que el constructor de la base sea inaccesible,
        // eso es un detalle de compilación (protected), pero el hecho de que la derivada funcione
        // confirma que el ctor protegido es válido.
    }
    #endregion

    #region Cache field behavior
    [Fact]
    public void Cache_StaticField_IsNotNull()
    {
        ObjectCache cache = TestSingletonObjectCache.Cache;

        Assert.NotNull(cache);
    }

    [Fact]
    public void Cache_StaticField_IsMemoryCacheDefaultInstance()
    {
        ObjectCache cache = TestSingletonObjectCache.Cache;

        Assert.Same(MemoryCache.Default, cache);
    }

    [Fact]
    public void Cache_StaticField_ReturnsSameInstanceOnMultipleAccesses()
    {
        ObjectCache cache1 = TestSingletonObjectCache.Cache;
        ObjectCache cache2 = TestSingletonObjectCache.Cache;

        Assert.Same(cache1, cache2);
    }

    [Fact]
    public void Cache_IsSharedAcrossMultipleDerivedInstances()
    {
        ObjectCache cacheFromInstance1 = TestSingletonObjectCache.Cache;
        ObjectCache cacheFromInstance2 = TestSingletonObjectCache.Cache;

        Assert.Same(cacheFromInstance1, cacheFromInstance2);
    }
    #endregion
}