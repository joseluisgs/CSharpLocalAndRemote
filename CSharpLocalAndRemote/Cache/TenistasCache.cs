using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Cache;

public class TenistasCacheImpl : CacheGeneric<long, Tenista>, ITenistasCache
{
    public TenistasCacheImpl(int cacheSize) : base(cacheSize)
    {
    }
}