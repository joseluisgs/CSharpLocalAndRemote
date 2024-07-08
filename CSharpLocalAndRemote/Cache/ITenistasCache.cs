using CSharpLocalAndRemote.model;

namespace CSharpLocalAndRemote.Cache;

public interface ITenistasCache : ICache<long, Tenista>
{
    public const int TenistasCacheSize = 5;
}