using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace Util.System.Win.Registry {
  [TestClass]
  public class RegistryKeyInfoTests {
    [TestMethod]
    public void RegistryKeyInfoTest() {
      const string pathToKey = @"HKEY_CURRENT_USER\Any\Path";
      var registryKey = new RegistryKeyInfo(pathToKey);

      Assert.AreEqual(registryKey.Hive, RegistryHive.CurrentUser);
      Assert.AreEqual(registryKey.PathToKey, @"Any\Path");
    }
  }
}