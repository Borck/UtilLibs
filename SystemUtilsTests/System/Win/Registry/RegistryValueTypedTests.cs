using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using System.Globalization;


namespace Util.System.Win.Registry {
  [TestClass]
  public class RegistryValueTypedTests {
    [TestMethod]
    public void TestString() {
      const string value = "gdfhfghfghd";

      var valueTyped = new RegistryValueTyped(value);
      Assert.AreEqual(RegistryValueKind.String, valueTyped.Kind);
      Assert.AreEqual(value, valueTyped.Value);
    }

    [TestMethod]
    public void TestStringEmpty() {
      const string value = "";

      var valueTyped = new RegistryValueTyped(value);
      Assert.AreEqual(RegistryValueKind.String, valueTyped.Kind);
      Assert.AreEqual(value, valueTyped.Value);
    }

    [TestMethod]
    public void TestHex() {
      const string value = "hex:00,01,02,03,04,05";

      var valueTyped = new RegistryValueTyped(value);
      Assert.AreEqual(RegistryValueKind.Binary, valueTyped.Kind);
      CollectionAssert.AreEqual(new byte[] { 0, 1, 2, 3, 4, 5 }, (byte[])valueTyped.Value);
    }


    [TestMethod]
    public void TestDword() {
      const string value = "dword:001e8480";

      var valueTyped = new RegistryValueTyped(value);

      var expected = int.Parse("001e8480", NumberStyles.HexNumber);
      Assert.AreEqual(RegistryValueKind.DWord, valueTyped.Kind);
      Assert.AreEqual(expected, valueTyped.Value);
    }

    [TestMethod]
    public void EqualsTest_String() {
      const string value = "shgjdhgjdgd";
      var registryValueTyped1 = new RegistryValueTyped(value);
      var registryValueTyped2 = new RegistryValueTyped(value);

      var @equals = registryValueTyped1.Equals(registryValueTyped2);
      Assert.IsTrue(@equals);
    }

    [TestMethod]
    public void EqualsTest_Binary() {
      const string value = "hex:00,01,02,03,04,05";
      var registryValueTyped1 = new RegistryValueTyped(value);
      var registryValueTyped2 = new RegistryValueTyped(value);

      var @equals = registryValueTyped1.Equals(registryValueTyped2);
      Assert.IsTrue(@equals);
    }

    [TestMethod]
    public void EqualsTest_DWord() {
      const string value = "dword:001e8480";
      var registryValueTyped1 = new RegistryValueTyped(value);
      var registryValueTyped2 = new RegistryValueTyped(value);

      var @equals = registryValueTyped1.Equals(registryValueTyped2);
      Assert.IsTrue(@equals);
    }

    [TestMethod]
    public void EqualsTest_QWord() {
      const string value = "qword:001e8480001e8480";
      var registryValueTyped1 = new RegistryValueTyped(value);
      var registryValueTyped2 = new RegistryValueTyped(value);

      var @equals = registryValueTyped1.Equals(registryValueTyped2);
      Assert.IsTrue(@equals);
    }

  }
}