using System;
using System.Diagnostics;

namespace Util.System.Win {
  public class ProcessEx {
    public enum States {
      Running,
      Ready,
      Unknown
    };

    private readonly Process _proc;

    public ProcessEx(int processId, string commandline) {
      _proc=Process.GetProcessById(processId);
      CommandLine=commandline;
    }

    public string CommandLine { get; }

    public States State {
      get {
        try {
          return _proc.Responding ? States.Ready : States.Running;
        } catch (Exception) {
          return States.Unknown;
        }
      }
    }
  }
}