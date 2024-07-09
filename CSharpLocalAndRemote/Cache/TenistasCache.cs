using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Cache;

public class TenistasCacheImpl(int cacheSize) : CacheGeneric<long, Tenista>(cacheSize), ITenistasCache;