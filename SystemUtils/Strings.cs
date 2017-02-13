using System;
using System.Text;

namespace Util {
  public static class Strings {
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

    /// <summary>
    /// Converts string from snake_case to CamelCase.
    /// </summary>
    /// <param name="str">the string to convert</param>
    /// <returns>the camel case string</returns>
    public static string ToCamelCaseFromSnakeCase(string str) {
      var tokens = str.Split('_');

      var camel = new StringBuilder();

      foreach (var atom in tokens) {
        if (atom.Length<=0)
          continue;
        camel.Append(char.ToUpper(atom[0]));
        if (atom.Length>1)
          camel.Append(atom.Substring(1).ToLower());
      }
      return camel.ToString();
    }

  }
}