using System;
using System.Text;

namespace Util {
  public static class Stringg {
    /// <summary>
    ///   Separates the string in two parts at the position of the first appearance of the separator. The Separator part will
    ///   be excluded. If no separator was found the tail is null.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="separator"></param>
    /// <param name="tail"></param>
    /// <returns>head of the string</returns>
    public static string SplitOnceAt(this string a, char separator, out string tail) {
      var i = a.IndexOf(separator);
      if (i==-1) {
        tail=null;
        return a;
      }
      tail=i+1<a.Length ? a.Substring(i+1) : "";
      return a.Substring(0, i);
    }

    /// <summary>
    ///   Separates the string in two parts at the position of the first appearance of the separator. The Separator part will
    ///   be excluded. If no separator was found the tail is null.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="separator"></param>
    /// <param name="tail"></param>
    /// <param name="stringComparison"></param>
    /// <returns>head of the string</returns>
    public static string SplitOnceAt(this string a, string separator, out string tail, StringComparison stringComparison) {
      var i = a.IndexOf(separator, stringComparison);
      if (i==-1) {
        tail=null;
        return a;
      }
      tail=i+1<a.Length ? a.Substring(i+1) : "";
      return a.Substring(0, i);
    }

    /// <summary>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="replaceChars"></param>
    /// <param name="replacement"></param>
    /// <returns></returns>
    public static string Replace(this string source, char[] replaceChars, char replacement) {
      var s = new StringBuilder();
      var j = 0;
      var i = source.IndexOfAny(replaceChars);
      while (i!=-1) {
        s.Append(source.Substring(j, i-j));
        s.Append(replacement);
        j=i;
        i=source.IndexOfAny(replaceChars, j+1);
      }
      s.Append(source.Substring(j));
      return s.ToString();
    }
  }
}