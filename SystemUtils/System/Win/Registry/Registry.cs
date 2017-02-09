using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Util.System.Win.Registry {
  public static class Registry {
    #region constants

    internal const string HKCR = "HKEY_CLASSES_ROOT";
    internal const string HKCR2 = "HKCR";
    internal const string HKCU = "HKEY_CURRENT_USER";
    internal const string HKCU2 = "HKCU";
    internal const string HKLM = "HKEY_LOCAL_MACHINE";
    internal const string HKLM2 = "HKLM";
    internal const string HKU = "HKEY_USERS";
    internal const string HKPD = "HKEY_PERFORMANCE_DATA";
    internal const string HKCC = "HKEY_CURRENT_CONFIG";
    internal const string HKDD = "HKEY_DYNAMIC_DATA";
    internal const string HKDD2 = "HKEY_DYN_DATA";

    private static readonly Dictionary<string, RegistryHive> Str2Hive =
      new Dictionary<string, RegistryHive>(StringComparer.OrdinalIgnoreCase) {
        {HKCR, RegistryHive.ClassesRoot},
        {HKCR2, RegistryHive.ClassesRoot},
        {HKCU, RegistryHive.CurrentUser},
        {HKCU2, RegistryHive.CurrentUser},
        {HKLM, RegistryHive.LocalMachine},
        {HKLM2, RegistryHive.LocalMachine},
        {HKU, RegistryHive.Users},
        {HKPD, RegistryHive.PerformanceData},
        {HKCC, RegistryHive.CurrentConfig}
      };

    private static readonly Dictionary<string, RegistryKey> Str2HiveKey =
      new Dictionary<string, RegistryKey>(StringComparer.OrdinalIgnoreCase) {
        {HKCR, Microsoft.Win32.Registry.ClassesRoot},
        {HKCR2, Microsoft.Win32.Registry.ClassesRoot},
        {HKCU, Microsoft.Win32.Registry.CurrentUser},
        {HKCU2, Microsoft.Win32.Registry.CurrentUser},
        {HKLM, Microsoft.Win32.Registry.LocalMachine},
        {HKLM2, Microsoft.Win32.Registry.LocalMachine},
        {HKU, Microsoft.Win32.Registry.Users},
        {HKPD, Microsoft.Win32.Registry.PerformanceData},
        {HKCC, Microsoft.Win32.Registry.CurrentConfig}
      };

    private static readonly Dictionary<string, string> ExpandKeys = new Dictionary<string, string> {
      {HKCR, HKCR},
      {HKCR2, HKCR},
      {HKCU, HKCU},
      {HKCU2, HKCU},
      {HKLM, HKLM},
      {HKLM2, HKLM},
      {HKU, HKU},
      {HKPD, HKPD},
      {HKCC, HKCC}
    };

    #endregion

    #region Parsing

    /**
     * return the hive and writes the rest path back to regPath
     */

    public static string CutHiveString(ref string pathToKey) {
      var i = pathToKey.IndexOf('\\');
      string result;
      if (i!=-1) {
        if (!ExpandKeys.TryGetValue(pathToKey.Substring(0, i), out result))
          throw new ArgumentException("No registry hive found", nameof(pathToKey));
        pathToKey=i+1<pathToKey.Length ? pathToKey.Substring(i+1) : "";
      } else {
        if (!ExpandKeys.TryGetValue(pathToKey, out result))
          throw new ArgumentException("No registry hive found", nameof(pathToKey));
        pathToKey="";
      }
      return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RegistryHive GetHive(string key) {
      var i = key.IndexOf('\\');
      RegistryHive result;
      if (Str2Hive.TryGetValue(i!=1 ? key.Substring(0, i) : key, out result))
        return result;
      throw new ArgumentException($"Unknown hive in Key \"{key}\"");
    }

    public static RegistryKey OpenKey(string key) {
      var i = key.IndexOf('\\');
      RegistryKey result;

      if (i!=1) {
        if (!Str2HiveKey.TryGetValue(key.Substring(0, i), out result))
          throw new ArgumentException($"Unknown hive in Key \"{key}\"");
        if (i+1<key.Length)
          result=result.OpenSubKey(key.Substring(i+1));
      } else if (!Str2HiveKey.TryGetValue(key, out result))
        throw new ArgumentException($"Unknown hive in Key \"{key}\"");
      return result;
    }

    #endregion
  }
}