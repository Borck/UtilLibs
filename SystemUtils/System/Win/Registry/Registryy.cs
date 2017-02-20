using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Util.System.Win.Registry {
  public static class Registryy {
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

    /// <summary>
    /// Parses the registry hive from the registry hive name
    /// </summary>
    /// <param name="regHiveName"></param>
    /// <returns>the registry hive</returns>
    public static RegistryHive GetRegistryHive(string regHiveName) {
      switch (regHiveName) {
        case "HKCR":
          return RegistryHive.ClassesRoot;
        case "HKCU":
          return RegistryHive.CurrentUser;
        case "HKLM":
          return RegistryHive.LocalMachine;
        case "HKU":
          return RegistryHive.Users;
        case "HKCC":
          return RegistryHive.CurrentConfig;
        case "HKPD":
          return RegistryHive.PerformanceData;
        default:
          const string prefix = "HKEY_";
          if (!regHiveName.StartsWith(prefix))
            throw new ArgumentException("This is not a valid registry hive: "+regHiveName);

          var regValueKindNameCamel = Strings.ToCamelCaseFromSnakeCase(regHiveName.Remove(0, prefix.Length));
          return (RegistryHive)Enum.Parse(typeof(RegistryHive), regValueKindNameCamel);
      }


    }


    public static RegistryValueKind GetRegistryValueKind(string regValueKindName) {
      const string prefix = "REG_";

      if (!regValueKindName.StartsWith(prefix))
        throw new ArgumentException("This is not a valid registry value kind: "+regValueKindName);

      var regValueKindNameCamel = Strings.ToCamelCaseFromSnakeCase(regValueKindName.Remove(0, prefix.Length));
      return (RegistryValueKind)Enum.Parse(typeof(RegistryValueKind), regValueKindNameCamel);
    }

    /// <summary>
    /// Opens a RegistryKey from a path "<RegistryHive>\path\to\key". I.e. "HKEY_CURRENT_USER\Software\Microsoft"
    /// </summary>
    /// <param name="pathToKey"></param>
    /// <param name="forceKey"></param>
    /// <returns></returns>
    public static Optional<RegistryKey> OpenRegistryKey(string pathToKey, bool forceKey = false) {
      var registryKeyInfo = new RegistryKeyInfo(pathToKey);
      return OpenRegistryKey(registryKeyInfo, forceKey);
    }

    public static Optional<RegistryKey> OpenRegistryKey(RegistryKeyInfo registryKeyInfo, bool forceKey = false) {

      var baseKey = RegistryKey.OpenBaseKey(registryKeyInfo.Hive, RegistryView.Default);
      var aRegKey = registryKeyInfo.PathToKey;

      return forceKey
        ? Optional<RegistryKey>.Of(baseKey.CreateSubKey(aRegKey))
        : Optional<RegistryKey>.OfNullable(baseKey.OpenSubKey(aRegKey));
    }



    public static RegistryKey OpenParent(this RegistryKey registryKey) {
      var pathToRegKey = registryKey.Name;
      var lastSep = pathToRegKey.LastIndexOf('\\');
      var pathToParent = pathToRegKey.Substring(0, lastSep);
      var keyInfoParent = new RegistryKeyInfo(pathToParent);
      return OpenRegistryKey(keyInfoParent).Value;
    }

    public static string GetNameWithoutPath(this RegistryKey registryKey) {
      var pathToRegKey = registryKey.Name;
      var lastSep = pathToRegKey.LastIndexOf('\\');
      return pathToRegKey.Substring(lastSep+1);
    }


    public static void Delete(this RegistryKey registryKey) {
      using (var parent = registryKey.OpenParent()) {
        var nameWithoutPath = registryKey.GetNameWithoutPath();

        registryKey.Close();
        parent.DeleteSubKeyTree(nameWithoutPath);
      }
    }

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