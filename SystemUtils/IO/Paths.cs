using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Util.IO {
  public static class Paths {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AddToken(this List<string> dest, string a, int fromIn, int toEx) {
      if (fromIn<toEx)
        dest.Add(a.Substring(fromIn, toEx-fromIn));
    }

    /// <summary>
    ///   Separates the filter to its tokens depending of the positions of wildcards and separates the token of the drive. Cuts
    ///   the token which defines the drive specification. Splits on any double asterisk ('**'). Trims last directory
    ///   separator.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="drive"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string[] CreateTokens([NotNull] string filter, out string drive) {
      if (filter.Length<=2) {
        drive=filter;
        return new string[0];
      }

      var fromIn = filter.IndexOf(Path.DirectorySeparatorChar, 2);
      if (fromIn==-1)
        return RECU.Equals(drive=filter) ? new[] { filter } : new string[0];

      drive=filter.Substring(0, fromIn);
      var result = new List<string>();
      var toEx = filter[filter.Length-1]!=Path.DirectorySeparatorChar ? filter.Length : filter.Length-1;
      for (var i = ++fromIn; i<toEx;) {
        switch (filter[i]) {
          case ANY_WORD:
            if (i+1<filter.Length&&filter[i+1]==ANY_WORD) {
              // recursion
              result.AddToken(filter, fromIn, i-1); //create token of all uncaptured before
              result.Add(RECU); //create token of **
              fromIn=i+=RECU.Length+1;
              break;
            }

            //word placeholder found
            var iSep1 = filter.IndexOf(DirSep, i+1);
            if (iSep1==-1)
              goto End; //end reached
            result.AddToken(filter, fromIn, iSep1);
            fromIn=i=iSep1+1;
            break;
          case ANY_CHAR:
            //character placeholder found
            var iSep2 = filter.IndexOf(DirSep, i+1);
            if (iSep2==-1)
              goto End; //end reached
            result.AddToken(filter, fromIn, iSep2);
            fromIn=i=iSep2+1;
            break;
          default:
            i++;
            break;
        }
      }
      End:
      result.AddToken(filter, fromIn, toEx); //add last token if there is any

      return result.ToArray();
    }

    #region Nested class

    private struct TokenIterator {
      private readonly string _filter;
      private readonly string[] _strs;
      private readonly int _pos;
      private int _i;
      private readonly int _dirCount;

      /// <summary>
      /// </summary>
      /// <param name="tokens"></param>
      /// <param name="filter">source filter</param>
      /// <param name="isFile">is file search</param>
      /// <param name="position">position in source filter (for debugging purpose)</param>
      public TokenIterator(string[] tokens, string filter, bool isFile, int position = 0) {
        _filter=filter;
        _pos=position;
        _strs=tokens;
        _i=-1;
        _dirCount=isFile ? tokens.Length-1 : tokens.Length;
      }


      private TokenIterator(TokenIterator ti, int increment) {
        _strs=ti._strs;
        _i=ti._i+increment;
        _filter=ti._filter;
        _pos=ti._pos;
        _dirCount=ti._dirCount;
      }

      public string Last() {
        return _strs.Last();
      }

      public string Next() {
        return _strs[++_i];
      }

      public bool HasNext() {
        return _i+1<_dirCount;
      }

      public TokenIterator PreviousIterator() {
        return new TokenIterator(this, -1);
      }

      public TokenIterator Copy() {
        return new TokenIterator(this, 0);
      }
    }

    #endregion

    #region Static fields

    private static char DirSep => Path.DirectorySeparatorChar;
    private static char VolSep => Path.VolumeSeparatorChar;

    private const char ANY_CHAR = '?';
    private const string S_ANY_CHAR = "?";
    private const char ANY_WORD = '*';
    private const string S_ANY_WORD = "*";
    private static readonly string DoubleDirSep = DirSep + DirSep.ToString();
    private const string RECU = S_ANY_WORD + S_ANY_WORD;

    private static readonly char[] WildCards = {ANY_CHAR, ANY_WORD};

    private static readonly Regex LocalDriveRegex = new Regex($"^([a-zA-Z])\\{VolSep}$");
    private static readonly Regex NetDriveRegex = new Regex(@"^(\\\\[\w\-\s\.]+)$");

    #endregion

    #region drives

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static HashSet<string> GetLocalDrives() {
      return new HashSet<string>(DriveInfo.GetDrives().Select(di => di.Name.Substring(0, di.Name.Length-1)),
        StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    ///   Create the tokens of those filters which it's drive exists.
    /// </summary>
    /// <param name="filters"> directory filters</param>
    /// <param name="fileSearch"> indicates a file search, last token will be ignored for directory search</param>
    /// <returns>Directory of TokenIterators grouped by there drive name</returns>
    private static Collections.Lookup<string, TokenIterator> CreateTokensGroupedByDrive(IEnumerable<string> filters,
      bool fileSearch) {
      HashSet<string> localDrives = null;
      var result = new Collections.Lookup<string, TokenIterator>();

      foreach (var dirfilter in filters) {
        string drive;
        var tokens = CreateTokens(dirfilter, out drive);

        if ((S_ANY_CHAR+VolSep).Equals(drive)) {
          // all local drives
          foreach (var ld in localDrives??(localDrives=GetLocalDrives()))
            result.Add(ld, new TokenIterator(tokens, dirfilter, fileSearch, RECU.Length+1));
        } else {
          Match match;
          if ((match=LocalDriveRegex.Match(drive)).Success) {
            // local drive
            if (!(localDrives??(localDrives=GetLocalDrives())).Contains(match.Value))
              continue;
            result.Add(drive, new TokenIterator(tokens, dirfilter, fileSearch, match.Index+match.Length));
          } else if ((match=NetDriveRegex.Match(drive)).Success) {
            // network drive
            var posNext = match.Index+match.Length;
            if (Directory.Exists(match.Value))
              result.Add(drive,
                new TokenIterator(dirfilter.Substring(posNext+1).Split(DirSep), dirfilter, fileSearch, posNext));
          }
        }
      }
      return result;
    }

    #endregion

    #region directories

    /// <summary>
    /// </summary>
    /// <param name="filterMap"></param>
    /// <returns></returns>
    private static Collections.Lookup<string, TokenIterator> DoGetDirectories(Collections.Lookup<string, TokenIterator> filterMap) {
      var result = new Collections.Lookup<string, TokenIterator>();

      var filterMapIn = new Collections.Lookup<string, TokenIterator>();
      var searches = new Collections.Lookup<string, TokenIterator>();

      while (!filterMap.IsEmpty()) {
        foreach (var path in filterMap.Keys.ToArray()) {
          var pds = path+DirSep;

          var pathAdded = false;
          foreach (var it in filterMap.Take(path)) {
            if (!it.HasNext()) {
              if (!pathAdded) {
                result.Add(path, it);
                pathAdded=true;
              }
              continue;
            }

            var token = it.Next();
            var outStr = "Testing \""+path+'|'+token+"\"";
            Trace.WriteLine(outStr);

            if (RECU.Equals(token)) {
              // recursive folder traversation
              searches.Add(S_ANY_WORD, it.PreviousIterator());
              if (!it.HasNext()) {
                if (!pathAdded) {
                  result.Add(path, it);
                  pathAdded=true;
                }
                continue;
              }
              token=it.Next();
            }
            searches.Add(token, it);
          }

          //search for existing directories//

          if (searches.ContainsKey(S_ANY_WORD)) {
            // full search - list all directories directly in path
            string[] foundedPaths;
            try {
              foundedPaths=Directory.GetDirectories(path+DirSep, S_ANY_WORD);
            } catch (UnauthorizedAccessException e) {
              Log.Log.Warn("Cannot search directories in directory.", e);
              continue;
            }

            var foundedNames = Array.ConvertAll(foundedPaths, Path.GetFileName);

            foreach (var dirNamePattern in searches.Keys.ToArray()) {
              var its = searches.Take(dirNamePattern);

              if (dirNamePattern.IndexOfAny(WildCards)==-1) {
                // pattern without wildcards
                if (foundedNames.Contains(dirNamePattern))
                  filterMapIn.AddAll(pds+dirNamePattern, its);
                continue;
              }

              if (S_ANY_WORD.Equals(dirNamePattern)) {
                // fullsearch -> add all iterators to each path with moved index
                foreach (var foundedPath in foundedPaths)
                  foreach (var it in its)
                    filterMapIn.Add(foundedPath, it.Copy());
                continue;
              }

              var regex =
                new Regex("^"+Regex.Escape(dirNamePattern).Replace(S_ANY_WORD, ".*").Replace(S_ANY_CHAR, ".")+"$");
              foreach (var found in foundedNames)
                if (regex.IsMatch(found))
                  foreach (var it in its)
                    filterMapIn.Add(pds+found, it.Copy());
            }
          } else // non full search
            foreach (var dirNamePattern in searches.Keys.ToArray()) {
              var its = searches.Take(dirNamePattern);
              try {
                var dirEnum = Directory.GetDirectories(pds, dirNamePattern).Cast<string>().GetEnumerator();
                if (!dirEnum.MoveNext())
                  continue;

                filterMapIn.AddAll(dirEnum.Current, its);
                while (dirEnum.MoveNext())
                  filterMapIn.AddAll(dirEnum.Current, its.Select(it => it.Copy()));
              } catch (UnauthorizedAccessException e) {
                Log.Log.Warn("Cannot search directories in directory.", e);
              }
            }
        }

        Debug.Assert(filterMap.IsEmpty());
        Debug.Assert(searches.IsEmpty());
        Objects.Swap(ref filterMap, ref filterMapIn);
      }

      return result;
    }

    public static string[] GetDirectories(ICollection<string> filters) {
      filters=NormalizeFilters(filters);
      CheckDirectoryFilters(filters);

      var pathMap = CreateTokensGroupedByDrive(filters, false);
      pathMap=DoGetDirectories(pathMap);
      return pathMap.Keys.ToArray();
    }

    /// <summary>
    ///   Represents the filters to search directories.
    ///   <para />
    ///   Allowed statements are:
    ///   <list type="table">
    ///     <listheader>
    ///       <term>term</term>
    ///       <description>description</description>
    ///     </listheader>
    ///     <item>
    ///       <term>%NAME%</term>
    ///       <description>Environmental variable</description>
    ///     </item>
    ///     <item>
    ///       <term>?</term>
    ///       <description>Placeholder for one character out of [a-Z0-9_.\s] (regex)</description>
    ///     </item>
    ///     <item>
    ///       <term>*</term>
    ///       <description>Placeholder for one word out of all combinations of ? and the empty word, like ?* (regex)</description>
    ///     </item>
    ///     <item>
    ///       <term>**</term>
    ///       <description>In addition to * allows directory separators [/\] (regex) and therefore directory recursion</description>
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="filters">paths </param>
    /// <returns></returns>
    public static string[] GetDirectories(string filters) {
      return GetDirectories(filters.Split(Path.PathSeparator));
    }

    #endregion

    #region files

    private static string[] DoGetFiles(Collections.Lookup<string, TokenIterator> filterMap) {
      var files = new Collections.Lookup<string, string>(StringComparer.OrdinalIgnoreCase,
        StringComparer.OrdinalIgnoreCase);
      foreach (var directory in filterMap.Keys.ToArray())
        files.AddAll(directory, filterMap.Take(directory).Select(it => it.Last()));
      foreach (var directory in files.Keys.ToArray()) //TODO: check if keys changes in loop
        foreach (var file in files.Take(directory))
          try {
            files.AddAll(directory, Directory.GetFiles(directory, file));
          } catch (UnauthorizedAccessException) {
            //ignore
          }


      var dirs = files.Keys.ToList();
      dirs.Sort();

      var result = new string[files.Count()];
      var i = 0;
      foreach (var file in dirs.SelectMany(dir => files.Take(dir)))
        result[i++]=file;
      return result;
    }

    public static string[] GetFiles(ICollection<string> filters) {
      filters=NormalizeFilters(filters);
      CheckFileFilters(filters);

      var pathMap = CreateTokensGroupedByDrive(filters, true);
      pathMap=DoGetDirectories(pathMap);
      return DoGetFiles(pathMap);
    }

    public static string[] GetFiles(string filters) {
      return GetFiles(filters.Split(Path.PathSeparator));
    }

    #endregion

    #region conditional filter checks

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CheckDirectoryFilter([NotNull] string filter) {
      if (string.IsNullOrEmpty(filter))
        throw new FormatException(nameof(filter)+" is empty or null.");

      if (filter.StartsWith(RECU))
        throw new FormatException("Filter starts with recursion '**' is not allow. \""+filter+"\"");

      if (filter.Length<2||(!filter.StartsWith(DoubleDirSep)&&filter[1]!=VolSep))
        throw new FormatException("Drive missing in filter: "+filter);

      /*
              if(filter.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                throw new FormatException("Invalid character found. " + filter);
      */

      //check recursion
      var i = filter.IndexOf(RECU, StringComparison.Ordinal);
      var n = filter.Length;
      for (; i!=-1; i=filter.IndexOf(RECU, i+1, StringComparison.Ordinal)) {
        // check format '\\**\\', @begin: '**\\', @end: '\\**'
        if ((i>=1&&filter[i-1]!=DirSep)||
            (i+RECU.Length<filter.Length&&filter[i+RECU.Length]!=DirSep))
          throw new FormatException("Recursion not in format '"+DirSep+RECU+DirSep+"': "+filter);

        // check for illegal recursion pairs '**\\**'
        if (i+RECU.Length*2<n&&RECU.Equals(filter.Substring(i+RECU.Length+1, RECU.Length)))
          throw new FormatException("Recursion pairing '"+RECU+DirSep+RECU+
                                    "' is not allowed. Use only one recursion. "+filter);
      }
      if (n>2&&filter.IndexOf(DirSep+DirSep.ToString(), 2, StringComparison.Ordinal)!=-1)
        throw new FormatException("Illegal sequence of multiply directory separators found: "+filter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CheckDirectoryFilters(IEnumerable<string> filters) {
      foreach (var filter in filters)
        CheckDirectoryFilter(filter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CheckFileFilters(IEnumerable<string> filters) {
      foreach (var filter in filters) {
        CheckDirectoryFilter(filter);
        var i = filter.LastIndexOf(DirSep);
        if (i==-1)
          throw new FormatException("Filter is not a file filter. "+filter);
        if (i==filter.Length-1)
          throw new FormatException("File filter contains illegal directory separator at the end. "+filter);
        if (filter.EndsWith(RECU))
          throw new FormatException("File filter contains illegal directory recursion at the end. "+filter);
      }
    }

    /// <summary>
    ///   Normalizes the filters by expanding environmental variables, trims spaces and unifies the directory separators.
    /// </summary>
    /// <param name="filters"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string[] NormalizeFilters(ICollection<string> filters) {
      var result = new string[filters.Count];
      var i = 0;
      foreach (var filter in filters)
        result[i++]=Environment.ExpandEnvironmentVariables(filter.Trim())
          .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
      return result;
    }

    #endregion

    #region path operations

    /// <summary>
    ///   We developed a custom implementation of the Path GetFileNameWithoutExtension method.
    ///   Info: The routine is over three times faster but has slight behavioral differences.The speed helps when this method
    ///   is often called.
    ///   Example for different behaviour: 'c:\cdf.\adf' returns 'cdf'
    ///   Path.GetFileNameWithoutExtension: 1064 ns
    ///   GetFileNameWithoutExtensionFast:   321 ns (unmodified)
    ///   Source: http://www.dotnetperls.com/path
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetFileNameWithoutExFast(string value) {
      // Find last available character.
      // ... This is either last index or last index before last period.
      var lastIndex = value.Length-1;
      for (var i = lastIndex; i>=1; i--) {
        if (value[i]!='.')
          continue;
        lastIndex=i-1;
        break;
      }
      // Find first available character.
      // ... Is either first character or first character after closest /
      // ... or \ character after last index.
      var firstIndex = 0;
      for (var i = lastIndex-1; i>=0; i--) {
        switch (value[i]) {
          case '/':
          case '\\': {
              firstIndex=i+1;
              goto End;
            }
        }
      }
      End:
      // Return substring.
      return value.Substring(firstIndex, (lastIndex-firstIndex+1));
    }

    public static string GetPathAndFileNameWithoutExt(string value) {
      // Find last available character.
      // ... This is either last index or last index before last period.
      for (var i = value.Length-1; i>=1; i--) {
        switch (value[i]) {
          case '\\':
          case '/':
            return value;
          case '.':
            return value.Substring(0, i);
        }
      }
      return value;
    }

    #endregion
  }
}