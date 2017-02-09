using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Util.IO {
  [TestClass]
  public class PathsTest {

    #region path tests

    [TestMethod]
    public void GetFullPathWithoutExt() {
      Assert.AreEqual(@"c:\\cdf.\adf", Paths.GetPathAndFileNameWithoutExt(@"c:\\cdf.\adf"));
      Assert.AreEqual(@"c:\\cdf.\adf", Paths.GetPathAndFileNameWithoutExt(@"c:\\cdf.\adf."));
      Assert.AreEqual(@"c:\\cdf.\adf", Paths.GetPathAndFileNameWithoutExt(@"c:\\cdf.\adf.ad"));
      Assert.AreEqual(@"c:\\cdf.\adf.ad", Paths.GetPathAndFileNameWithoutExt(@"c:\\cdf.\adf.ad.fd"));
      Assert.AreEqual(@"\adf", Paths.GetPathAndFileNameWithoutExt(@"\adf"));
      Assert.AreEqual(@"\adf", Paths.GetPathAndFileNameWithoutExt(@"\adf."));
      Assert.AreEqual(@"\adf", Paths.GetPathAndFileNameWithoutExt(@"\adf.ad"));
    }

    #endregion

    #region directory tests

    private static string[] GetDirectoriesContains([NotNull]string expected, string paths) {
      var dirs = Paths.GetDirectories(paths);
      expected=Environment.ExpandEnvironmentVariables(expected);
      Assert.IsTrue(dirs.Length>=1, "Found at least one directory.");
      if (!dirs.Any(dir => expected.Equals(dir, StringComparison.OrdinalIgnoreCase)))
        Assert.Fail("Expected directory not found.");
      return dirs;
    }

    private static void GetDirectoriesContains(string expected, string paths, int founds) {
      var dirs = GetDirectoriesContains(expected, paths);
      Assert.IsTrue(dirs.Length==founds, "Found directories at the given amount.");
    }

    private static void GetDirectoriesFails(string paths) {
      try {
        Paths.GetDirectories(paths);
        Assert.Fail("Illegal paths does not cause an exception.");
      } catch (FormatException) {
        // ignored
      }
    }

    [TestMethod]
    public void GetDirectories() {
      GetDirectoriesContains(@"c:", @"c:", 1);
      GetDirectoriesContains(@"c:\Windows", @"c:/Windows\", 1);
      GetDirectoriesContains(@"c:\Windows\system32", @"?:/Wi*ws\s?s*e*m32", 1);
      GetDirectoriesContains(@"c:\Windows\system32", @"c:\*\system32", 1);
      GetDirectoriesContains(@"C:\Windows\System32\spp\tokens\skus\csvlk-pack", @"C:\Windows\System32\spp\**\csvlk-pack", 1);
      GetDirectoriesContains(@"C:\Windows\System32\spp\tokens\skus\csvlk-pack", @"C:\Windows\System32\spp\**");
      GetDirectoriesContains(@"c:\Windows", @"c:\*");
      GetDirectoriesContains(@"%USERPROFILE%", @"%USERPROFILE%");
      GetDirectoriesContains(@"c:\Windows", @"c:\Windows; c:\Not existing folder; c:\      /", 1);

      GetDirectoriesFails(@"");
      GetDirectoriesFails(@"c");
      GetDirectoriesFails(@"c:\\\\");
      GetDirectoriesFails(@"c:\\%");

      GetDirectoriesFails(@"c:\**\**");
      GetDirectoriesFails(@"c:\**\**\");
      GetDirectoriesFails(@"c:\**\**\ab");
      GetDirectoriesFails(@"**");
      GetDirectoriesFails(@"**\");
      GetDirectoriesFails(@";");
    }

    [TestMethod]
    public void GetDirectoriesStressTest() {
      GetDirectoriesContains(@"%USERPROFILE%\Documents", @"%USERPROFILE%\**");
    }

    #endregion

    #region file tests

    private static string[] GetFilesContains([NotNull]string expected, string paths) {
      var dirs = Paths.GetFiles(paths);
      expected=Environment.ExpandEnvironmentVariables(expected);
      Assert.IsTrue(dirs.Length>=1, "Found at least one directory.");
      if (!dirs.Any(dir => expected.Equals(dir, StringComparison.OrdinalIgnoreCase)))
        Assert.Fail("Expected directory not found.");
      return dirs;
    }

    private static void GetFilesContains(string expected, string paths, int founds) {
      var dirs = GetFilesContains(expected, paths);
      Assert.IsTrue(dirs.Length==founds, "Found directories at the given amount.");
    }

    private static void GetFilesFails(string paths) {
      try {
        Paths.GetFiles(paths);
        Assert.Fail("Illegal paths does not cause an exception.");
      } catch (FormatException) {
        // ignored
      }
    }

    [TestMethod]
    public void GetFiles() {
      GetFilesContains(@"c:\Windows\explorer.exe", @"c:\*\*.exe");

      GetFilesFails(@"");
      GetFilesFails(@"c");
      GetFilesFails(@"c:");
      GetFilesFails(@"c:\\\\");
      GetFilesFails(@"c:\\%");

      GetFilesFails(@"c:\**");
      GetFilesFails(@"c:\gfdfgd\**/");
      GetFilesFails(@"c:\**/");
      GetFilesFails(@"c:\**\**");
      GetFilesFails(@"c:\**\**\");
      GetFilesFails(@"c:\**\**\ab");
      GetFilesFails(@"**");
      GetFilesFails(@"**\");
      GetFilesFails(@"c:");
      GetFilesFails(@";");
    }

    #endregion
  }
}