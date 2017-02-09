using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace Util.IO {
  public class FileSearchPattern {
    private const string XML_VERSION = "1.0";
    private const string XML_ENCODING = "UTF-8";

    private const string SEP = "/";
    private const string ROOT = "files";
    private const string FILE_NODE = "file";
    private const string KEY_NODE = "key";
    private const string FILENAME_NODE = "filename";
    private const string PATH_NODE = "path";

    private const string APP_ROOT_PATH = SEP + ROOT + SEP + FILE_NODE;

    public FileSearchPattern(string key, string[] pathPatterns, string filenamePattern) {
      Key=key;
      PathPattern=pathPatterns;
      FilenamePattern=filenamePattern;
    }

    public string Key { get; }
    public string[] PathPattern { get; }
    public string FilenamePattern { get; }

    public static FileSearchPattern[] Load(string xmlPath) {
      var xml = new XmlDocument();
      xml.LoadXml(xmlPath);


      var apps = xml.SelectNodes(APP_ROOT_PATH);
      if (apps==null)
        throw new NullReferenceException(ROOT);

      try {
        var result = new List<FileSearchPattern>();
        foreach (XmlNode appNodes in apps) {
          var keyNode = appNodes.SelectSingleNode(KEY_NODE);
          if (keyNode==null)
            throw new XmlSchemaException("key not found");

          var fileNode = appNodes.SelectSingleNode(FILE_NODE);
          if (fileNode==null)
            throw new XmlSchemaException("file not found");

          var key = keyNode.Value;
          var filename = fileNode.Value;

          var pathNodes = appNodes.SelectNodes(PATH_NODE);
          if (pathNodes==null)
            throw new XmlSchemaException("no path found");

          var paths = (from XmlNode pathNode in pathNodes select pathNode.Value).ToArray();

          result.Add(new FileSearchPattern(key, paths, filename));
        }
        return result.ToArray();
      } catch (Exception e) {
        //TODO:MessageBox.Show("Error in reading XML", "xmlError", MessageBoxButtons.OK);
        Log.Log.Err("Cannot load from xml.", e);
      }
      return null;
    }

    public void Save(string xmlPath) {
      Save(xmlPath, new[] { this });
    }

    internal static void Save(string xmlPath, FileSearchPattern[] patterns) {
      /*
            if (patterns.Length == 0)
              throw new Exception("pattern list empty");
      */
      // TODO: check, if empty pattern list possible (yes?!)

      var doc = new XmlDocument();
      var appRoot = CreateXmlBody(doc);

      foreach (var pattern in patterns)
        AppendFilePattern(doc, appRoot, pattern);

      doc.Save(xmlPath);
    }


    private static void AppendFilePattern(XmlDocument doc, XmlNode fileRoot, FileSearchPattern pattern) {
      // TODO: check, if exceptions required
      /*
            if (pattern.PathPattern.Length == 0)
              throw new ArgumentException("pattern not valid: emply path pattern");
      */

      var fileNode = doc.CreateElement(FILE_NODE);
      fileRoot.AppendChild(fileNode);

      var keyNode = doc.CreateElement(KEY_NODE);
      keyNode.Value=pattern.Key;
      fileRoot.AppendChild(keyNode);

      var filenameNode = doc.CreateElement(FILENAME_NODE);
      filenameNode.Value=pattern.FilenamePattern;
      fileRoot.AppendChild(filenameNode);

      foreach (var path in pattern.PathPattern) {
        var pathNode = doc.CreateElement(PATH_NODE);
        pathNode.Value=path;
        fileRoot.AppendChild(pathNode);
      }
    }

    private static XmlNode CreateXmlBody(XmlDocument doc) {
      var xmlDeclaration = doc.CreateXmlDeclaration(XML_VERSION, XML_ENCODING, null);
      var root = doc.DocumentElement;
      doc.InsertBefore(xmlDeclaration, root);
      var appsRoot = doc.CreateElement(ROOT);
      doc.AppendChild(appsRoot);
      return appsRoot;
    }
  }
}