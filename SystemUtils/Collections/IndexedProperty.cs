using System;

namespace Util.Collections {
  public class IndexedProperty<T, I> {
    private readonly Func<I, T> _getter;
    private readonly Action<I, T> _setter;

    public IndexedProperty(Func<I, T> getter, Action<I, T> setter) {
      if (getter==null||setter==null)
        throw new ArgumentNullException();
      _getter=getter;
      _setter=setter;
    }

    public T this[I index] {
      get { return _getter(index); }
      set { _setter(index, value); }
    }
  }
}