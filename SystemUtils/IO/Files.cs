using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Util.IO {
  /// <summary>
  ///   FileManager
  /// </summary>
  public static class Files {
    /// <summary>
    ///   Liefert den Inhalt der Datei zurück.
    /// </summary>
    /// <param name="filepath">Dateipfad</param>
    public static List<string> ReadFile(string filepath) {
      var lines = new List<string>();
      FileStream fsInput = null;
      try {
        fsInput=new FileStream(filepath, FileMode.Open, FileAccess.Read);
        using (var srInput = new StreamReader(fsInput, Encoding.Default)) {
          string line;
          while ((line=srInput.ReadLine())!=null)
            lines.Add(line);
        }
      } finally {
        fsInput?.Dispose();
      }
      return lines;
    }

    /// <summary>
    ///   Schreibt den übergebenen Inhalt in eine Textdatei.
    /// </summary>
    /// <param name="filepath">Pfad zur Datei</param>
    /// <param name="lines">zu schreibender Text</param>
    public static void WriteFile(string filepath, string lines) {
      using (var myFile = new StreamWriter(filepath, false, Encoding.Default))
        myFile.Write(lines);
    }

    /// <summary>
    ///   Fügt den übergebenen Text an das Ende einer Textdatei an.
    /// </summary>
    /// <param name="filepath">Pfad zur Datei</param>
    /// <param name="lines">anzufügender Text</param>
    public static void Append(string filepath, string lines) {
      using (var myFile = new StreamWriter(filepath, true))
        myFile.Write(lines);
    }

    /// <summary>
    ///   Liefert den Inhalt der übergebenen Zeilennummer einer Textdatei zurück.
    /// </summary>
    /// <param name="filepath">Pfad zur Textdatei</param>
    /// <param name="line">Zeilennummer</param>
    public static string ReadLine(string filepath, int line) {
      var sContent = "";
      float fRow = 0;
      if (!File.Exists(filepath))
        return sContent;

      using (var myFile = new StreamReader(filepath, Encoding.Default))
        while (!myFile.EndOfStream&&fRow++<line)
          sContent=myFile.ReadLine();
      if (fRow<line)
        sContent="";
      return sContent;
    }

    /// <summary>
    ///   Schreibt den übergebenen Text in eine definierte Zeile einer Txtdatei.
    /// </summary>
    /// <param name="filepath">Pfad zur Textdatei</param>
    /// <param name="iLine">Zeilennummer</param>
    /// <param name="lines">Text für die übergebene Zeile</param>
    /// <param name="replace">Text in dieser Zeile überschreiben (t) oder einfügen (f)</param>
    public static void WriteLine(string filepath, int iLine, string lines, bool replace) {
      var sContent = "";
      string[] delimiterstring = { "\r\n" };

      if (File.Exists(filepath))
        using (var myFile = new StreamReader(filepath, Encoding.Default))
          sContent=myFile.ReadToEnd();

      var sCols = sContent.Split(delimiterstring, StringSplitOptions.None);

      //TODO: use StringBuilder
      if (sCols.Length>=iLine) {
        if (!replace)
          sCols[iLine-1]=lines+"\r\n"+sCols[iLine-1];
        else
          sCols[iLine-1]=lines;

        sContent="";
        for (var x = 0; x<sCols.Length-1; x++) {
          sContent+=sCols[x]+"\r\n";
        }
        sContent+=sCols[sCols.Length-1];
      } else {
        for (var x = 0; x<iLine-sCols.Length; x++)
          sContent+="\r\n";
        sContent+=lines;
      }


      var mySaveFile = new StreamWriter(filepath);
      mySaveFile.Write(sContent);
      mySaveFile.Close();
    }

    public static bool OpenExplorer(string path) {
      if (!Directory.Exists(path))
        return false;

      Process.Start("explorer.exe", path);
      return true;
    }
  }
}