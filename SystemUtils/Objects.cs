using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Util {
  public static class Objectt {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap<T>(ref T lhs, ref T rhs) {
      var temp = lhs;
      lhs=rhs;
      rhs=temp;
    }

    /// <summary>
    ///   Type-safe equals
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj1"></param>
    /// <param name="obj2"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T>(T obj1, T obj2) {
      return EqualityComparer<T>.Default.Equals(obj1, obj2);
    }
  }
}