using JetBrains.Annotations;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using Util.IO;

namespace Util.System.Win.Registry {
  public class AppPathMonitor {
    private const string ROOT_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
    private static readonly RegistryKey AppPathRoot = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(ROOT_PATH);
    private readonly RegistryKey _key;

    private readonly SortedDictionary<string, object> _properties;
    public readonly string CurrValue;

    public readonly string Name;
    private FileSearchPattern _fileSearch;

    private RegistryMonitor _rm;

    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="dest"></param>
    /// <param name="properties">collection of keys and its value</param>
    /// <param name="dest1"></param>
    public AppPathMonitor([NotNull] string name, [NotNull] string dest, IDictionary<string, object> properties,
      string dest1) {
      //if (properties.Count == 0)
      //  throw new ArgumentException("map coult not be empty");

      //TODO:
      /*
            if (!FileSearch.IsValidFileName(name))
              throw new ArgumentException("Invalid filename", name);
            if (!FileSearch.IsValidPath(dest))
              throw new ArgumentException("Invalid filepath", dest);
      */

      Name=name;
      Dest=dest1;
      _properties=new SortedDictionary<string, object>(properties, StringComparer.CurrentCultureIgnoreCase);

      _key=AppPathRoot.OpenSubKey(Name);


      if (_key==null) {
        // good, key not exists
        _key=AppPathRoot.CreateSubKey(Name);
        if (_key!=null) {
          // TODO: error handling
          return;
        }
        var msg = "Could not create registry key (missing permissions?): "+RegPath;
        Log.Log.Err(msg);
        throw new Exception(msg);
      }

      //
      Log.Log.Warn("RegistryKey '"+RegPath+"' already exists");
      CurrValue=ReadDefaultVal();

      // TODO: hier gehts weiter
    }

    public string Dest { get; }

    private string RegPath => AppPathRoot.Name+"\\"+Name;

    private string ReadDefaultVal() {
      return _key.GetValue(null).ToString();
    }

    private void WriteDefaultVal() {
      Log.Log.Info("Default value of '"+RegPath+"' changed to "+Dest);
      _key.SetValue(null, Dest);
    }


    private void EnableNotify() {
      if (_rm==null)
        _rm=new RegistryMonitor(_key+"\\@");
      _rm.Start();
    }

    private void DisableNotify() {
      _rm.Stop();
    }

    private RegistryKey GetKey() {
      return AppPathRoot.OpenSubKey(Name);
    }


    public void SetControl(FileSearchPattern pattern) {
      if (_fileSearch==null) {
        if (pattern==null)
          return;

        EnableNotify();
      } else {
        if (pattern==null)
          DisableNotify();
      }

      _fileSearch=pattern;
    }

    private void RegKeyChanged() {
    }


    public static AppPathMonitor[] GetAllRegisteredApps() {
      var keys = AppPathRoot.GetSubKeyNames();

      var result = new List<AppPathMonitor>();
      var map = new SortedDictionary<string, object>();
      foreach (var key in keys) {
        var currentRoot = AppPathRoot.OpenSubKey(key);

        if (currentRoot==null)
          throw new NullReferenceException("AppRegistration.GetAllRegisteredApps()");

        Console.WriteLine("Hint: #SubValueNames"+currentRoot.GetValueNames().Length);

        var valNames = currentRoot.GetValueNames();
        if (valNames.Length<=0)
          continue;

        foreach (var valName in valNames)
          map.Add(valName, currentRoot.GetValue(valName));

        throw new NotSupportedException();
        //result.Add(new AppRegistration(key, map));
      }
      return result.ToArray();
    }

    public void Register() {
      if (AppPathRoot.OpenSubKey(Name)!=null)
        AppPathRoot.DeleteSubKeyTree(Name);

      var appRoot = AppPathRoot.CreateSubKey(Name);
      if (appRoot==null)
        throw new Exception("No Permission to create a key in the Registry ("+ROOT_PATH+")");

      foreach (var entry in _properties)
        appRoot.SetValue(entry.Key, entry.Value);
    }
  }
}