using Microsoft.Win32;
using System;

namespace Util.System.Win.Registry {
  public class RegistryKeyInfo {
    public readonly RegistryHive Hive;
    public readonly string PathToKey;

    public RegistryKeyInfo(RegistryHive hive, string pathToKey) {
      Hive=hive;
      PathToKey=pathToKey;
    }

    public RegistryKeyInfo(string pathToKey) {
      var aKeyParts = pathToKey.Split(new[] { '\\' }, 2);

      if (aKeyParts.Length!=2)
        throw new ArgumentException("Try to open invalid registry key: "+pathToKey);

      Hive=Registryy.GetRegistryHive(aKeyParts[0]);
      PathToKey=aKeyParts[1];

    }
  }
}
