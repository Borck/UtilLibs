using System;
using System.Collections.Generic;

namespace Util {
  /// <summary>
  /// A container object which may or may not contain a non-null value. If a value is present, isPresent() will return true and get() will return the value.
  ///
  /// Additional methods that depend on the presence or absence of a contained value are provided, such as orElse() (return a default value if value not present) and ifPresent() (execute a block of code if the value is present).
  /// 
  /// This is a value-based class; use of identity-sensitive operations(including reference equality (==), identity hash code, or synchronization) on instances of Optional may have unpredictable results and should be avoided.
  /// </summary>
  /// <typeparam name="T">The type of the non null value</typeparam>
  public struct Optional<T> {
    private readonly T _value;



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
    public T OrElse(T @default) {
      return IsPresent ? _value : @default;
    }

    public static explicit operator T(Optional<T> optional) {
      return optional.Value;
    }
    public static implicit operator Optional<T>(T value) {
      return new Optional<T>(value);
    }

    public Optional<TU> Map<TU>(Func<T, TU> mapping) {
      return IsPresent
        ? Optional<TU>.Of(mapping.Invoke(Value))
        : Optional<TU>.Empty();
    }

    public Optional<TOut> Select<TOut>(Func<T, TOut> selector) {
      return IsPresent
        ? new Optional<TOut>(selector(_value))
        : new Optional<TOut>();
    }

    public Optional<TOut> SelectMany<TOut>(Func<T, Optional<TOut>> bind) {
      return IsPresent
        ? bind(_value)
        : new Optional<TOut>();
    }


    public override int GetHashCode() {
      return EqualityComparer<T>.Default.GetHashCode(_value);
    }

    public override bool Equals(object obj) {
      if (obj is Optional<T>)
        return Equals((Optional<T>)obj);
      else
        return false;
    }

    public bool Equals(Optional<T> other) {
      return Equals(_value, other._value);
    }
  }
}