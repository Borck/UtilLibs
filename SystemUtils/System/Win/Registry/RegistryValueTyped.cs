using JetBrains.Annotations;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.Linq;

namespace Util.System.Win.Registry {
  public class RegistryValueTyped {
    private static readonly char[] TypeSeparator = { ':' };
    private static readonly char[] BinarySeparator = { ',' };

    private readonly string _valueString;
    public readonly RegistryValueKind Kind;
    public readonly object Value;


    public RegistryValueTyped(object value, RegistryValueKind kind) {
      Value=value;
      Kind=kind;

    }

    public RegistryValueTyped(string valueString) {
      _valueString=valueString;
      Value=GetValueAndType(valueString, out Kind);
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
        case "qword":
          return RegistryValueKind.QWord;
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

        case RegistryValueKind.QWord:
          return long.Parse(valueString, NumberStyles.HexNumber);

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

    public bool Equals([CanBeNull]RegistryValueTyped registryValue) {
      return registryValue!=null
             &&Objects.Equals(Kind, registryValue.Kind)
             &&EqualsValues(Value, registryValue.Value, Kind);
    }


    private static bool EqualsValues(object value1, object value2, RegistryValueKind kind) {
      if (value1==null)
        return value2==null;

      switch (kind) {
        case RegistryValueKind.String:
          return string.Equals(value1 as string, value2 as string);
        case RegistryValueKind.Binary:
          return ((byte[])value1).SequenceEqual(value2 as byte[]);
        case RegistryValueKind.DWord:
          return (int)value1==(int)value2;
        case RegistryValueKind.QWord:
          return (long)value1==(long)value2;
        default:
          throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
      }
    }

    /// <summary>
    /// Bestimmt, ob das angegebene Objekt mit dem aktuellen Objekt identisch ist.
    /// </summary>
    /// <returns>
    /// true, wenn das angegebene Objekt und das aktuelle Objekt gleich sind, andernfalls false.
    /// </returns>
    /// <param name="obj">Das Objekt, das mit dem aktuellen Objekt verglichen werden soll. </param><filterpriority>2</filterpriority>
    public override bool Equals(object obj) {
      return obj is RegistryValueTyped&&Equals((RegistryValueTyped)obj);
    }


    /// <summary>
    /// Fungiert als die Standardhashfunktion. 
    /// </summary>
    /// <returns>
    /// Ein Hashcode für das aktuelle Objekt.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override int GetHashCode() {
      unchecked {
        return ((int)Kind*397)^(Value?.GetHashCode()??0);
      }
    }
  }
}
