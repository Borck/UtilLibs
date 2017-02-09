using JetBrains.Annotations;
using Shell32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using Util.IO;

namespace Util.System.Win {
  public static class Environmentt {
    //public enum AutostartLocation {Shortcut, HKCU, HKLM, Any};

    private const string RUN_LOCATION = @"Software\Microsoft\Windows\CurrentVersion\Run";


    //----------------------------------------
    // needed references
    //----------------------------------------
    // Microsoft Shell Controls and Automation
    // Windows Script Host Object Model


    internal const uint DONT_RESOLVE_DLL_REFERENCES = 0x00000001;
    internal const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;


    private const string REGFILES_SEARCHMASK = "*(+).reg;*({0}+).reg;*({0}.{1}+).reg";
    private const string REGISTRY_FILENAME = "regedit.exe";

    public static string Username => WindowsIdentity.GetCurrent()?.Name;

    public static string GetOsVersionString(bool shortname = false) {
      var osInfo = Environment.OSVersion;
      var strVers = string.Empty;

      if (osInfo.Platform==PlatformID.Win32Windows) {
        // Windows 98 / 98SE oder Windows Me. Windows 95 unterstützt .NET nicht 
        if (osInfo.Version.Minor==10)
          strVers="Windows 98";
        if (osInfo.Version.Minor==90)
          strVers="Windows Me";
      }

      if (osInfo.Platform==PlatformID.Win32NT) {
        // Windows NT 4.0, 2000, XP oder Server 2003. Windows NT 3.51 unterstützt .NET nicht 
        switch (osInfo.Version.Major) {
          case 4: {
              strVers="Windows NT 4.0";
              break;
            }
          case 5: {
              switch (osInfo.Version.Minor) {
                case 0:
                  strVers="Windows 2000";
                  break;
                case 1:
                  strVers="Windows XP";
                  break;
                case 2:
                  strVers="Windows Server 2003";
                  break;
              }
              break;
            }
          case 6: {
              switch (osInfo.Version.Minor) {
                case 0:
                  strVers="Windows Vista";
                  break;
                case 1:
                  strVers="Windows 7";
                  break;
                case 2:
                  strVers="Windows 8";
                  break;
                case 3:
                  strVers="Windows 8.1";
                  break;
              }
              break;
            }
        }
      }
      if (strVers=="") {
        strVers="Windows";
      } else {
        var sp = osInfo.ServicePack;
        if (shortname) {
          strVers=strVers.Replace("Windows ", "Win");
          strVers+=" "+sp.Replace("Service Pack ", "SP");
          // +", Revision " + osInfo.Version.Revision.ToString() + ", " + osInfo.VersionString;
        } else {
          strVers+=" "+sp;
        }
      }

      return strVers;
    }

    public static string GetShortcutTarget(string shortcutFilename) {
      var pathOnly = Path.GetDirectoryName(shortcutFilename);
      var filenameOnly = Path.GetFileName(shortcutFilename);

      var shell = new Shell();
      var folder = shell.NameSpace(pathOnly);
      var folderItem = folder.ParseName(filenameOnly);

      if (folderItem==null)
        return string.Empty; // Not found

      var link =
        (ShellLinkObject)folderItem.GetLink;
      return link.Path;
    }

    /// <summary>
    ///   Returns whether auto start is enabled.
    /// </summary>
    /// <param name="keyName">Registry Key Name</param>
    /// <param name="assemblyLocation">Assembly location (e.g. Assembly.GetExecutingAssembly().Location)</param>
    public static bool IsAutostartEnabled(string keyName, string assemblyLocation) {
      var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RUN_LOCATION);
      var value = (string)key?.GetValue(keyName);
      return value!=null&&value==assemblyLocation;
    }

    /// <summary>
    ///   Sets the autostart value for the assembly.
    /// </summary>
    /// <param name="keyName">Registry Key Name</param>
    /// <param name="assemblyLocation">Assembly location (e.g. Assembly.GetExecutingAssembly().Location)</param>
    public static bool SetAutostart(string keyName, string assemblyLocation) {
      var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
      var result = key!=null;
      if (result)
        key.SetValue(keyName, assemblyLocation);
      return result;
    }

    /// <summary>
    ///   Unsets the autostart value for the assembly.
    /// </summary>
    /// <param name="keyName">Registry Key Name</param>
    public static bool UnsetAutostart(string keyName) {
      var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
      var result = key!=null;
      if (result)
        key.DeleteValue(keyName);
      return result;
    }

    public static string GetLocalizedName([NotNull] string resource) {
      if (resource==null)
        throw new ArgumentNullException(nameof(resource));
      var id = 0;
      string file;

      if (resource.StartsWith("@"))
        resource=resource.Substring(1);

      var idPos = resource.LastIndexOf(",", StringComparison.Ordinal);

      if (idPos!=0) {
        file=Environment.ExpandEnvironmentVariables(resource.Substring(0, idPos));
        id=int.Parse(resource.Substring(idPos+1));
        id=Math.Abs(id);
      } else {
        file=resource;
      }

      return GetLocalizedName(file, id);
    }

    public static string GetLocalizedName(string file, int id) {
      var sb = new StringBuilder(500);
      var res = "";
      var len = sb.Capacity;

      var hMod = UnsafeNativeMethods.LoadLibraryEx(file, IntPtr.Zero,
        DONT_RESOLVE_DLL_REFERENCES|LOAD_LIBRARY_AS_DATAFILE);

      if (hMod==IntPtr.Zero)
        return res;

      if (UnsafeNativeMethods.LoadString(hMod, id, sb, len)!=0) {
        res=sb.ToString();
      }

      UnsafeNativeMethods.FreeLibrary(hMod);

      return res;
    }

    //------------------------------------------------------
    // Führt die REG-Dateien im Unterordner 'regfiles' in der Registrierungsdatenbank zusammen.
    public static int MergeRegFiles(string root) {
      var result = -1;
      var osInfo = Environment.OSVersion;

      // Suchmuster und Pfad der Reg-File festlegen
      var pattern = string.Format(REGFILES_SEARCHMASK,
        osInfo.Version.Major, osInfo.Version.Minor);
      var files = Paths.GetFiles(Path.Combine(root, pattern));
      var lines = new List<string>();
      foreach (var file in files)
        lines.AddRange(Files.ReadFile(file));

      // Reg-File in temporäre Datei zusammenfassen und diese an Regedit.exe übergeben
      var path = Path.GetTempFileName();
      Files.WriteFile(path, string.Join("\n", lines.ToArray()));
      var regedit = Process.Start(REGISTRY_FILENAME, "/s "+@path);

      // TODO: is that statement ok
      if (regedit==null)
        return result;
      regedit.WaitForExit();
      result=regedit.ExitCode;

      return result;
    }

    public static void RunRegedit() {
      var regedit = new Process { StartInfo={ FileName=REGISTRY_FILENAME, Arguments="" } };
      regedit.Start();
    }

    public static void RunInCmd(string cmdLine, bool runHidden = false) {
      var process = new Process();
      var startInfo = new ProcessStartInfo {
        WindowStyle=runHidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal,
        FileName="cmd.exe",
        Arguments=cmdLine
        //Arguments = "/C copy /b Image1.jpg + Archive.rar Image2.jpg"
      };
      process.StartInfo=startInfo;
      process.Start();
    }

    private static class UnsafeNativeMethods {
      [DllImport("user32.dll", CharSet = CharSet.Unicode)]
      internal static extern int LoadString(IntPtr hModule, int resourceId, StringBuilder resourceValue, int len);

      //[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "LoadLibraryW")]
      //internal static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);
      [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
      internal static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

      [DllImport("kernel32.dll", ExactSpelling = true)]
      internal static extern int FreeLibrary(IntPtr hModule);
    }
  }
}