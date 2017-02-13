using Microsoft.Win32;
using System;
using System.Globalization;
using System.Linq;

namespace Util.System.Win.Registry {
  public class RegistryValueTyped {
    private static readonly char[] TypeSeparator = { ':' };
    private static readonly char[] BinarySeparator = { ',' };

    public readonly RegistryValueKind Type;
    public readonly object Value;


    public RegistryValueTyped(string value) {
      Value=GetValueAndType(value, out Type);
    }

    private static object GetValueAndType(string value, out RegistryValueKind valueType) {
      var tokens = value.Split(TypeSeparator, 2);
      valueType=GetType(tokens);
      return GetValue(tokens, valueType);
    }

    private static RegistryValueKind GetType(string[] tokens) {
      if (tokens.Length<2)
        return RegistryValueKind.String;
      switch (tokens[0].ToLower()) {
        case "hex":
          return RegistryValueKind.Binary;
        case "dword":
          return RegistryValueKind.DWord;
        default:
          throw new ArgumentException("Registry value type is not known: "+tokens[0]);
      }
    }


    private static object GetValue(string[] tokens, RegistryValueKind type) {
      string valueString = tokens.Length>0
        ? tokens.Last()
        : "";

      switch (type) {
        case RegistryValueKind.String:
          return valueString;

        case RegistryValueKind.DWord:
          return int.Parse(valueString, NumberStyles.HexNumber);

        case RegistryValueKind.Binary:
          var binaryTokens = valueString.Split(BinarySeparator);

          var bytes = new byte[binaryTokens.Length];
          var i = 0;
          foreach (var binaryToken in binaryTokens)
            bytes[i++]=byte.Parse(binaryToken, NumberStyles.HexNumber);

          return bytes;

        default:
          throw new NotSupportedException("Registry value type is not supported: "+type);

      }
    }

  }
}
