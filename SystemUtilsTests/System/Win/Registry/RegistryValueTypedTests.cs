using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System.Globalization;

namespace Util.System.Win.Registry {
  [TestClass()]
  public class RegistryValueTypedTests {
    [TestMethod()]
    public void TestString() {
      const string value = "gdfhfghfghd";

      var valueTyped = new RegistryValueTyped(value);
      Assert.AreEqual(RegistryValueKind.String, valueTyped.Type);
      Assert.AreEqual(value, valueTyped.Value);
    }

    [TestMethod()]
    public void TestStringEmpty() {
      const string value = "";

      var valueTyped = new RegistryValueTyped(value);
      Assert.AreEqual(RegistryValueKind.String, valueTyped.Type);
      Assert.AreEqual(value, valueTyped.Value);
    }

    [TestMethod()]
    public void TestHex() {
      const string value = "hex:00,01,02,03,04,05";

      var valueTyped = new RegistryValueTyped(value);
      Assert.AreEqual(RegistryValueKind.Binary, valueTyped.Type);
      CollectionAssert.AreEqual(new byte[] { 0, 1, 2, 3, 4, 5 }, (byte[])valueTyped.Value);
    }


    [TestMethod()]
    public void TestDword() {
      const string value = "dword:001e8480";

      var valueTyped = new RegistryValueTyped(value);

      var expected = int.Parse("001e8480", NumberStyles.HexNumber);
      Assert.AreEqual(RegistryValueKind.DWord, valueTyped.Type);
      Assert.AreEqual(expected, valueTyped.Value);
    }
  }
}