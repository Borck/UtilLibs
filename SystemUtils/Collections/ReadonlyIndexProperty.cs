using System;

namespace Util.Collections {
  public class ReadonlyIndexedProperty<TIndex, TData> {
    private readonly Func<TIndex, TData> _getter;

    public ReadonlyIndexedProperty(Func<TIndex, TData> getter) {
      if (getter==null)
        throw new ArgumentNullException();
      _getter=getter;
    }

    public TData this[TIndex index] => _getter(index);
  }
}