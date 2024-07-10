using CSharpLocalAndRemote.Logger;

namespace CSharpLocalAndRemote.Cache;

public class CacheGeneric<TK, T> : ICache<TK, T> where TK : notnull
{
    // Lista enlazada para mantener el orden de los elementos en la cache
    private readonly LinkedList<(TK key, T value)> _cacheList; // Lista enlazada de nodos para mantener el LRU

    // Mapa de claves a nodos de la lista enlazada y la busqueda de elementos en O(1)
    private readonly Dictionary<TK, LinkedListNode<(TK key, T value)>> _cacheMap;
    private readonly int _cacheSize;

    private readonly Serilog.Core.Logger _logger = LoggerUtils<CacheGeneric<TK, T>>.GetLogger();

    public CacheGeneric(int cacheSize)
    {
        _cacheSize = cacheSize;
        _cacheMap = new Dictionary<TK, LinkedListNode<(TK key, T value)>>(_cacheSize);
        _cacheList = new LinkedList<(TK key, T value)>();
    }

    public T? Get(TK key)
    {
        _logger.Debug("Obteniendo valor de la cache para la clave {key}", key);
        if (!_cacheMap.TryGetValue(key, out var node))
            // Key
            return default;

        // Movemos el nodo al frente (más recientemente usado)
        _cacheList.Remove(node);
        _cacheList.AddFirst(node);

        return node.Value.value;
    }

    public void Put(TK key, T value)
    {
        _logger.Debug("Insertando valor en la cache para la clave {key}", key);
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

            node = new LinkedListNode<(TK key, T value)>((key, value));
            _cacheMap[key] = node;
        }

        _cacheList.AddFirst(node);
    }

    public void Remove(TK key)
    {
        _logger.Debug("Eliminando valor de la cache para la clave {key}", key);
        if (!_cacheMap.TryGetValue(key, out var node)) return;
        _cacheList.Remove(node);
        _cacheMap.Remove(key);
    }

    public void Clear()
    {
        _logger.Debug("Limpiando la cache");
        _cacheMap.Clear();
        _cacheList.Clear();
    }

    public int Size()
    {
        _logger.Debug("Obteniendo el tamaño de la cache");
        return _cacheMap.Count;
    }

    public ISet<TK> Keys()
    {
        _logger.Debug("Obteniendo las claves de la cache");
        return new HashSet<TK>(_cacheMap.Keys);
    }

    public ICollection<T> Values()
    {
        _logger.Debug("Obteniendo los valores de la cache");
        var values = new List<T>();
        foreach (var (key, value) in _cacheList) values.Add(value);
        return values;
    }

    public bool ContainsKey(TK key)
    {
        _logger.Debug("Comprobando si la cache contiene la clave {key}", key);
        return _cacheMap.ContainsKey(key);
    }

    public bool ContainsValue(T value)
    {
        _logger.Debug("Comprobando si la cache contiene el valor {value}", value);
        foreach (var (key, val) in _cacheList)
            if (EqualityComparer<T>.Default.Equals(val, value))
                return true;
        return false;
    }

    public bool IsEmpty()
    {
        _logger.Debug("Comprobando si la cache está vacía");
        return _cacheMap.Count == 0;
    }
}