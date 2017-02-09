using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Util.System.Win.Registry {
  public static class Extensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InvokeIfRequired(this Dispatcher thisDispatcher, Action callback) {
      if (thisDispatcher.CheckAccess())
        callback();
      else
        thisDispatcher.Invoke(callback);
    }
  }
}