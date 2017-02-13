using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Util {
  [TestClass()]
  public class StringsTests {
    public void ToCamelCaseFromSnakeCaseTest() {
      const string testSample = "A_TEST_SAMPLE";

      var aConvertedSample = Strings.ToCamelCaseFromSnakeCase(testSample);
      Assert.AreEqual("ATestSample", aConvertedSample);
    }
  }
}