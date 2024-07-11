using CSharpLocalAndRemote.Cache;

namespace CSharpLocalAndRemote.Settings;

public class CacheSettings
{
    public int Size { get; set; } = ITenistasCache.TenistasCacheSize;
}