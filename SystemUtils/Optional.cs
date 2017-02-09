using System;

namespace Util {
  public class Optional<T> {
    private readonly T _value;

    private Optional() {
      _value=default(T);
    }


    private Optional(T value) {
      _value=value;
    }

    public T Value {
      get {
        if (_value==null)
          throw new InvalidOperationException("Value is not present");
        return _value;
      }
    }


    public bool IsPresent => _value!=null;

    public void IfPresent(Action<T> action) {
      if (IsPresent)
        action(_value);
    }

    public T OrElse(T defaultValue) {
      return IsPresent ? _value : defaultValue;
    }

    public Optional<TU> Map<TU>(Func<T, TU> mapping) {
      return IsPresent
        ? Optional<TU>.Of(mapping.Invoke(Value))
        : Optional<TU>.Empty();
    }

    public static Optional<T> Of(T value) {
      if (value==null)
        throw new ArgumentNullException();
      return new Optional<T>(value);
    }

    public static Optional<T> OfNullable(T value) {
      return new Optional<T>(value);
    }

    public static Optional<T> Empty() {
      return new Optional<T>();
    }
  }
}