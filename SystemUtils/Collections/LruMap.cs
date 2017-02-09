using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Util.Collections {
  public class LruMap<TK, TV> {
    private readonly Dictionary<TK, LinkedListNode<LruMapItem<TK, TV>>> _cacheMap =
      new Dictionary<TK, LinkedListNode<LruMapItem<TK, TV>>>();

    private readonly int _capacity;
    private readonly LinkedList<LruMapItem<TK, TV>> _lruList = new LinkedList<LruMapItem<TK, TV>>();

    public LruMap(int capacity) {
      _capacity=capacity;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool TryGetValue(TK key, out TV value) {
      LinkedListNode<LruMapItem<TK, TV>> node;
      if (!_cacheMap.TryGetValue(key, out node)) {
        value=default(TV);
        return false;
      }
      value=node.Value.Value;
      _lruList.Remove(node);
      _lruList.AddLast(node);
      return true;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Add(TK key, TV val) {
      if (_cacheMap.Count>=_capacity) {
        // Remove from LRUPriority and cache
        _cacheMap.Remove(_lruList.First.Value.Key);
        _lruList.RemoveFirst();
      }
      var node = new LinkedListNode<LruMapItem<TK, TV>>(new LruMapItem<TK, TV>(key, val));
      _lruList.AddLast(node);
      _cacheMap.Add(key, node);
    }
  }
}