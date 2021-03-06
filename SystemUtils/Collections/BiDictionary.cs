﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Util.Collections {
  /// <summary>
  ///   This is a dictionary guaranteed to have only one of each value and key.
  ///   It may be searched either by TFirst or by TSecond, giving a unique answer because it is 1 to 1.
  /// </summary>
  /// <typeparam name="TFirst">The type of the "key"</typeparam>
  /// <typeparam name="TSecond">The type of the "value"</typeparam>
  public class BiDictionary<TFirst, TSecond> : IEnumerable<KeyValuePair<TFirst, TSecond>> {
    private readonly IDictionary<TFirst, TSecond> _firstToSecond;
    private readonly IDictionary<TSecond, TFirst> _secondToFirst;

    /// <summary>
    ///   The number of pairs stored in the dictionary
    /// </summary>
    public int Count => _firstToSecond.Count;

    public IEnumerator<KeyValuePair<TFirst, TSecond>> GetEnumerator() {
      return _firstToSecond.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <summary>
    ///   Removes all items from the dictionary.
    /// </summary>
    public void Clear() {
      _firstToSecond.Clear();
      _secondToFirst.Clear();
    }

    #region

    public BiDictionary() {
      _firstToSecond=new Dictionary<TFirst, TSecond>();
      _secondToFirst=new Dictionary<TSecond, TFirst>();
    }

    public BiDictionary(int capacity) {
      _firstToSecond=new Dictionary<TFirst, TSecond>(capacity);
      _secondToFirst=new Dictionary<TSecond, TFirst>(capacity);
    }

    private BiDictionary(KeyValuePair<TFirst, TSecond>[] keyRelations) {
      _firstToSecond=keyRelations.ToDictionary(x => x.Key, x => x.Value);
      _secondToFirst=keyRelations.ToDictionary(x => x.Value, x => x.Key);
    }

    public static implicit operator BiDictionary<TFirst, TSecond>(KeyValuePair<TFirst, TSecond>[] keyRelations) {
      return new BiDictionary<TFirst, TSecond>(keyRelations);
    }

    #endregion

    #region Exception throwing methods

    /// <summary>
    ///   Tries to add the pair to the dictionary.
    ///   Throws an exception if either element is already in the dictionary
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    public void Add(TFirst first, TSecond second) {
      if (_firstToSecond.ContainsKey(first)||_secondToFirst.ContainsKey(second))
        throw new ArgumentException("Duplicate first or second");

      _firstToSecond.Add(first, second);
      _secondToFirst.Add(second, first);
    }

    /// <summary>
    ///   Find the TSecond corresponding to the TFirst first
    ///   Throws an exception if first is not in the dictionary.
    /// </summary>
    /// <param name="first">the key to search for</param>
    /// <returns>the value corresponding to first</returns>
    public TSecond GetByFirst(TFirst first) {
      TSecond second;
      if (!_firstToSecond.TryGetValue(first, out second))
        throw new ArgumentException("first");

      return second;
    }

    /// <summary>
    ///   Find the TFirst corresponing to the Second second.
    ///   Throws an exception if second is not in the dictionary.
    /// </summary>
    /// <param name="second">the key to search for</param>
    /// <returns>the value corresponding to second</returns>
    public TFirst GetBySecond(TSecond second) {
      TFirst first;
      if (!_secondToFirst.TryGetValue(second, out first))
        throw new ArgumentException("second");

      return first;
    }

    /// <summary>
    ///   Remove the record containing first.
    ///   If first is not in the dictionary, throws an Exception.
    /// </summary>
    /// <param name="first">the key of the record to delete</param>
    public void RemoveByFirst(TFirst first) {
      TSecond second;
      if (!_firstToSecond.TryGetValue(first, out second))
        throw new ArgumentException("first");

      _firstToSecond.Remove(first);
      _secondToFirst.Remove(second);
    }

    /// <summary>
    ///   Remove the record containing second.
    ///   If second is not in the dictionary, throws an Exception.
    /// </summary>
    /// <param name="second">the key of the record to delete</param>
    public void RemoveBySecond(TSecond second) {
      TFirst first;
      if (!_secondToFirst.TryGetValue(second, out first))
        throw new ArgumentException("second");

      _secondToFirst.Remove(second);
      _firstToSecond.Remove(first);
    }

    #endregion

    #region Try methods

    /// <summary>
    ///   Tries to add the pair to the dictionary.
    ///   Returns false if either element is already in the dictionary
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns>true if successfully added, false if either element are already in the dictionary</returns>
    public bool TryAdd(TFirst first, TSecond second) {
      if (_firstToSecond.ContainsKey(first)||_secondToFirst.ContainsKey(second))
        return false;

      _firstToSecond.Add(first, second);
      _secondToFirst.Add(second, first);
      return true;
    }


    /// <summary>
    ///   Find the TSecond corresponding to the TFirst first.
    ///   Returns false if first is not in the dictionary.
    /// </summary>
    /// <param name="first">the key to search for</param>
    /// <param name="second">the corresponding value</param>
    /// <returns>true if first is in the dictionary, false otherwise</returns>
    public bool TryGetSecond(TFirst first, out TSecond second) {
      return _firstToSecond.TryGetValue(first, out second);
    }

    /// <summary>
    ///   Find the TFirst corresponding to the TSecond second.
    ///   Returns false if second is not in the dictionary.
    /// </summary>
    /// <param name="second">the key to search for</param>
    /// <param name="first">the corresponding value</param>
    /// <returns>true if second is in the dictionary, false otherwise</returns>
    public bool TryGetFirst(TSecond second, out TFirst first) {
      return _secondToFirst.TryGetValue(second, out first);
    }

    /// <summary>
    ///   Remove the record containing first, if there is one.
    /// </summary>
    /// <param name="first"></param>
    /// <returns> If first is not in the dictionary, returns false, otherwise true</returns>
    public bool TryRemoveByFirst(TFirst first) {
      TSecond second;
      if (!_firstToSecond.TryGetValue(first, out second))
        return false;

      _firstToSecond.Remove(first);
      _secondToFirst.Remove(second);
      return true;
    }

    /// <summary>
    ///   Remove the record containing second, if there is one.
    /// </summary>
    /// <param name="second"></param>
    /// <returns> If second is not in the dictionary, returns false, otherwise true</returns>
    public bool TryRemoveBySecond(TSecond second) {
      TFirst first;
      if (!_secondToFirst.TryGetValue(second, out first))
        return false;

      _secondToFirst.Remove(second);
      _firstToSecond.Remove(first);
      return true;
    }

    #endregion
  }
}