using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;


namespace Util.System.Win.Registry {
  [TestClass()]
  public class RegistryyTests {

    [TestMethod()]
    public void OpenRegistryKeyTest() {
      const string pathToKey = @"HKEY_CURRENT_USER\Software";
      var registryKey = Registryy.OpenRegistryKey(pathToKey);
      Assert.AreEqual(pathToKey, registryKey.Name);
    }

    [TestMethod()]
    public void OpenParentKeyTest() {
      const string pathToParentKey = @"HKEY_CURRENT_USER\Software";
      const string pathToKey = pathToParentKey+@"\Microsoft";

      var registryKey = Registryy.OpenRegistryKey(pathToKey);
      var parentKey = registryKey.OpenParent();

      Assert.AreEqual(pathToParentKey, parentKey.Name);
    }

    [TestMethod()]
    public void GetNameWithoutPathTest() {
      const string pathToKey = @"HKEY_CURRENT_USER\Software\Microsoft";
      var registryKey = Registryy.OpenRegistryKey(pathToKey);

      var nameWithoutPath = registryKey.GetNameWithoutPath();
      Assert.AreEqual("Microsoft", nameWithoutPath);
    }

    [TestMethod()]
    public void GetRegistryHiveTest() {
      const string regHklm = "HKEY_LOCAL_MACHINE";

      var registryHive = Registryy.GetRegistryHive(regHklm);
      Assert.AreEqual(RegistryHive.LocalMachine, registryHive);
    }

    [TestMethod()]
    public void GetRegistryValueKindTest() {
      const string regBinary = "REG_BINARY";

      var registryValueKind = Registryy.GetRegistryValueKind(regBinary);
      Assert.AreEqual(RegistryValueKind.Binary, registryValueKind);
    }
  }
}