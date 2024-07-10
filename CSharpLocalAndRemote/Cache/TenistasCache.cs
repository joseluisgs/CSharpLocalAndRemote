using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Cache;

public class TenistasCache(int cacheSize) : CacheGeneric<long, Tenista>(cacheSize), ITenistasCache;