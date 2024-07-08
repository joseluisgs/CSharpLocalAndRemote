namespace CSharpLocalAndRemote.Cache;

public interface ICache<K, T>
{
    T? Get(K key);
    void Put(K key, T value);
    void Remove(K key);
    void Clear();
    int Size();
    ISet<K> Keys();
    ICollection<T> Values();
    bool ContainsKey(K key);
    bool ContainsValue(T value);
    bool IsEmpty();

    bool IsNotEmpty()
    {
        return !IsEmpty();
    }
}