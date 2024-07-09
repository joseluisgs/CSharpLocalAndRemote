namespace CSharpLocalAndRemote.Cache;

public interface ICache<TK, T>
{
    T? Get(TK key);
    void Put(TK key, T value);
    void Remove(TK key);
    void Clear();
    int Size();
    ISet<TK> Keys();
    ICollection<T> Values();
    bool ContainsKey(TK key);
    bool ContainsValue(T value);
    bool IsEmpty();

    bool IsNotEmpty()
    {
        return !IsEmpty();
    }
}