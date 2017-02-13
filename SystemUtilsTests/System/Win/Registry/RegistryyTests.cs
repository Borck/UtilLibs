using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace Util.System.Win.Registry {
  [TestClass()]
  public class RegistryyTests {
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