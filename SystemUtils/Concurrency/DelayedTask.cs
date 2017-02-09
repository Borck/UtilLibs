using System;
using System.Threading;
using System.Threading.Tasks;

namespace Util.Concurrency {
  public class DelayedTask {
    private readonly Action _action;
    private readonly Action _postAction;
    private bool _fired;

    public DelayedTask(Action action, Action postAction = null) {
      if (action==null)
        throw new ArgumentNullException(nameof(action));
      _action=action;
      _postAction=postAction;
    }

    /// <summary>
    /// </summary>
    /// <param name="delay">in milliseconds</param>
    /// <returns></returns>
    public void Start(int delay) {
      if (delay<0)
        throw new ArgumentOutOfRangeException(nameof(delay));

      (new Task(() => {
        Thread.Sleep(delay);
        lock (this) {
          var fired = _fired;
          _fired=true;
          if (!fired)
            _action();
        }
      })).Start();
    }

    /// <summary>
    /// </summary>
    /// <returns>task has fired</returns>
    public bool TryStop(Action postAction) {
      bool fired;
      lock (this) {
        fired=_fired;
        _fired=true;
      }

      if (fired)
        postAction?.Invoke();
      return fired;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public bool TryStop() {
      return TryStop(_postAction);
    }
  }
}