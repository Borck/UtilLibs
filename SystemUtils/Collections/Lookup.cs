using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Util.Collections {
  // Todo: better name
  public class Lookup<TKey, TElement> {
    private const int MaxDisposedCapacity = 1000;

    private readonly Dictionary<TKey, HashSet<TElement>> _dict;
    private readonly Stack<HashSet<TElement>> _disposed = new Stack<HashSet<TElement>>();

    private readonly IEqualityComparer<TElement> _valueComparer;
    private int _disposedCount;

    public Lookup() {
      _dict=new Dictionary<TKey, HashSet<TElement>>();
    }

    public Lookup(IEqualityComparer<TKey> keyComparer) {
      _dict=new Dictionary<TKey, HashSet<TElement>>(keyComparer);
      _valueComparer=EqualityComparer<TElement>.Default;
    }

    public Lookup(int capacity, [NotNull] IEqualityComparer<TKey> keyComparer) {
      _dict=new Dictionary<TKey, HashSet<TElement>>(capacity, keyComparer);
      _valueComparer=EqualityComparer<TElement>.Default;
    }

    public Lookup([NotNull] IEqualityComparer<TKey> keyComparer, [NotNull] IEqualityComparer<TElement> valueComparer) {
      _dict=new Dictionary<TKey, HashSet<TElement>>(keyComparer);
      _valueComparer=valueComparer;
    }

    public Lookup(int capacity, [NotNull] IEqualityComparer<TKey> keyComparer,
      [NotNull] IEqualityComparer<TElement> valueComparer) {
      _dict=new Dictionary<TKey, HashSet<TElement>>(capacity, keyComparer);
      _valueComparer=valueComparer;
    }

    public Lookup(int capacity) {
      _dict=new Dictionary<TKey, HashSet<TElement>>(capacity);
    }

    public Dictionary<TKey, HashSet<TElement>>.KeyCollection Keys => _dict.Keys;

    public bool Add(TKey key, TElement item) {
      HashSet<TElement> set;
      if (_dict.TryGetValue(key, out set))
        return set.Add(item);
      if (_disposedCount==0)
        set=new HashSet<TElement>(_valueComparer);
      else {
        _disposedCount--;
        set=_disposed.Pop();
      }
      _dict.Add(key, set);
      return set.Add(item);
    }

    public bool ContainsKey(TKey key) {
      return _dict.ContainsKey(key);
    }

    public void AddAll(TKey key, IEnumerable<TElement> items) {
      HashSet<TElement> set;
      var itemss = items as TElement[]??items.ToArray();

      if (!_dict.TryGetValue(key, out set)) {
        if (_disposedCount==0) {
          _dict.Add(key, new HashSet<TElement>(itemss, _valueComparer));
          return;
        }
        _disposedCount--;
        _dict.Add(key, set=_disposed.Pop());
      }
      set.UnionWith(itemss);
    }

    private void Disposit(HashSet<TElement> set) {
      set.Clear();
      if (_disposedCount>=MaxDisposedCapacity)
        return;
      _disposedCount++;
      _disposed.Push(set);
    }

    public bool TryGetValues(TKey key, out ICollection<TElement> values) {
      HashSet<TElement> set;
      if (!_dict.TryGetValue(key, out set)) {
        values=null;
        return false;
      }
      values=set.ToArray();
      return true;
    }

    public ICollection<TElement> Take(TKey key) {
      HashSet<TElement> set;
      if (!_dict.TryGetValue(key, out set))
        return null;
      var result = set.ToArray();
      _dict.Remove(key);
      Disposit(set);
      return result;
    }

    public bool Remove(TKey key) {
      HashSet<TElement> set;
      if (!_dict.TryGetValue(key, out set))
        return false;
      _dict.Remove(key);
      Disposit(set);
      return true;
    }

    /// <summary>
    ///   Returns true if it has no items, otherwise false.
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() {
      return !_dict.Any();
    }

    public bool Contains(TKey key, TElement item) {
      HashSet<TElement> set;
      return _dict.TryGetValue(key, out set)&&set.Contains(item);
    }

    /// <summary>
    ///   Counts the items.
    /// </summary>
    /// <returns></returns>
    public int Count() {
      return _dict.Values.Sum(value => value.Count);
    }

    /// <summary>
    ///   Counts the items.
    /// </summary>
    /// <returns></returns>
    public int CountKeys() {
      return _dict.Count;
    }

    public void ForEach(Action<TKey, TElement> action) {
      foreach (var key in _dict.Keys) {
        ICollection<TElement> elements;
        TryGetValues(key, out elements);
        foreach (var element in elements)
          action(key, element);
      }
    }
  }
}