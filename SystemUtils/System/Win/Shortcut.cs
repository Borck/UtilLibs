using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using File = System.IO.File;

namespace Util.System.Win {
  public class Shortcut {

    public readonly string Source;

    public Shortcut(string linkPath) {
      Source=linkPath;
    }


    public bool TryGetTarget(out string pathToTarget) {
      pathToTarget=((IWshShortcut)new WshShell().CreateShortcut(Source)).TargetPath;
      if (!File.Exists(pathToTarget)) {
        //the shortcut points to something invalid...
        var pathToTargetX86 = pathToTarget.Replace(" (x86)", "");
        if (!File.Exists(pathToTargetX86)) {
          pathToTarget=null;
          return false;
        }
        pathToTarget=pathToTargetX86;
      }
      return true;
    }

    public static List<Shortcut> GetStartmenuLink() {
      //C:\ProgramData\Microsoft\Windows\Start Menu\Programs
      string[] potentialFolders = {
        Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + @"\Programs",
        Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Programs"
      };
      return (from startmenufolder in potentialFolders
              from filename in Directory.GetFiles(startmenufolder, "*.lnk", SearchOption.AllDirectories)
              where Path.GetExtension(filename)==".lnk"
              select new Shortcut(filename)).ToList();
    }
  }
}
