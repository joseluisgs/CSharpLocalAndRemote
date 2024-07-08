using Serilog;
using Serilog.Core;

namespace CSharpLocalAndRemote.Cache;

public class CacheGeneric<K, T> : ICache<K, T>
{
    // Lista enlazada para mantener el orden de los elementos en la cache
    private readonly LinkedList<(K key, T value)> _cacheList; // Lista enlazada de nodos para mantener el LRU

    // Mapa de claves a nodos de la lista enlazada y la busqueda de elementos en O(1)
    private readonly Dictionary<K, LinkedListNode<(K key, T value)>> _cacheMap;
    private readonly int _cacheSize;
    private readonly Logger logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();

    public CacheGeneric(int cacheSize)
    {
        _cacheSize = cacheSize;
        _cacheMap = new Dictionary<K, LinkedListNode<(K key, T value)>>(_cacheSize);
        _cacheList = new LinkedList<(K key, T value)>();
    }

    public T? Get(K key)
    {
        logger.Debug("Obteniendo valor de la cache para la clave {key}", key);
        if (!_cacheMap.TryGetValue(key, out var node))
            // Key
            return default;

        // Movemos el nodo al frente (más recientemente usado)
        _cacheList.Remove(node);
        _cacheList.AddFirst(node);

        return node.Value.value;
    }

    public void Put(K key, T value)
    {
        logger.Debug("Insertando valor en la cache para la clave {key}", key);
        if (_cacheMap.TryGetValue(key, out var node))
        {
            // Actualizamos el valor existente y lo movemos al frente (más recientemente usado)
            _cacheList.Remove(node);
            node.Value = (key, value);
        }
        else
        {
            // Si la cache está llena, eliminamos el elemento menos recientemente usado
            if (_cacheMap.Count >= _cacheSize)
            {
                // Remove least recently used item
                var lru = _cacheList.Last;
                _cacheList.RemoveLast();
                _cacheMap.Remove(lru!.Value.key);
            }

            node = new LinkedListNode<(K key, T value)>((key, value));
            _cacheMap[key] = node;
        }

        _cacheList.AddFirst(node);
    }

    public void Remove(K key)
    {
        logger.Debug("Eliminando valor de la cache para la clave {key}", key);
        if (!_cacheMap.TryGetValue(key, out var node)) return;
        _cacheList.Remove(node);
        _cacheMap.Remove(key);
    }

    public void Clear()
    {
        _cacheMap.Clear();
        _cacheList.Clear();
    }

    public int Size()
    {
        return _cacheMap.Count;
    }

    public ISet<K> Keys()
    {
        return new HashSet<K>(_cacheMap.Keys);
    }

    public ICollection<T> Values()
    {
        var values = new List<T>();
        foreach (var (key, value) in _cacheList) values.Add(value);
        return values;
    }

    public bool ContainsKey(K key)
    {
        return _cacheMap.ContainsKey(key);
    }

    public bool ContainsValue(T value)
    {
        foreach (var (key, val) in _cacheList)
            if (EqualityComparer<T>.Default.Equals(val, value))
                return true;
        return false;
    }

    public bool IsEmpty()
    {
        return _cacheMap.Count == 0;
    }
}