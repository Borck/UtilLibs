using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace Util.System.Win {
  public class StartupMonitor {
    private readonly List<string> _candidates = new List<string>();
    private readonly List<ProcessEx> _procs = new List<ProcessEx>();
    public readonly int Amount;
    private int _currentCheck;

    public StartupMonitor() {
      var manager = new ManagementClass("Win32_StartupCommand");
      foreach (var strt in manager.GetInstances().Cast<ManagementObject>()) {
        if (strt["Location"].ToString()=="Startup") {
          //TODO: Autorun-Ordner, Lnk-Datei auslesen
        } else {
          _candidates.Add(strt["Command"].ToString());
        }
      }

      manager=new ManagementClass("Win32_Process");
      foreach (var strt in manager.GetInstances().Cast<ManagementObject>()) {
        string command;
        try {
          command=strt["CommandLine"].ToString();
        } catch (Exception) {
          command="";
        }

        try {
          var au = new ProcessEx(Convert.ToInt32(strt["ProcessID"]), command);
          _procs.Add(au);
        } catch (Exception) {
          // ignored
        }
      }

      AmountReady=0;
      Amount=_procs.Count;
      _currentCheck=Amount-1;
    }

    public int AmountReady { get; private set; }

    public void UpdateIterativ() {
      var count = Amount-AmountReady;
      if (count>0) {
        _currentCheck--;
        if (_currentCheck<0) {
          _currentCheck+=count;
        }
      }

      if (_procs[_currentCheck].State!=ProcessEx.States.Ready)
        return;

      _procs.RemoveAt(_currentCheck);
      AmountReady++;
    }
  }
}