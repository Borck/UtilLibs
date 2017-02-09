using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Util.Collections {
  public static class Extensions {
    public static void RemoveRange<T>(this IList<T> collection, int index, int count) {
      for (var i = index+count-1; i>=index; i--)
        collection.RemoveAt(i);
    }

    /// <summary>
    ///   Removes or add elements from/to the right side of the collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="newSize"></param>
    /// <param name="newEntry"></param>
    /// <returns>Size difference</returns>
    public static int Resize<T>(this IList<T> collection, int newSize, Func<T> newEntry) {
      var oldSize = collection.Count;
      var difference = newSize-oldSize;

      if (difference>0)
        for (var i = oldSize; i<newSize; i++)
          collection.Add(newEntry());
      else if (difference<0)
        collection.RemoveRange(oldSize, difference);
      return difference;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static T[][] Split<T>(this T[] array, int length) {
      // TODO: test
      var parts = array.Length/length;
      var rest = array.Length%length;

      //var tmpArray = array;

      var result = rest==0 ? new T[parts][] : new T[parts+1][];

      var i = 0;
      var pos = 0;
      if (parts>0)
        for (; i<parts; i++) {
          result[i]=new T[length];
          Array.Copy(array, pos, result[i], 0, length);
          pos+=length;
        }

      if (rest<=0)
        return result;

      result[i]=new T[rest];
      Array.Copy(array, pos, result[i], 0, rest);
      return result;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T[] Join<T>(this T[][] array) {
      // TODO: test it!
      var length = array.Sum(part => part.Length);
      var result = new T[length];

      var pos = 0;
      foreach (var part in array) {
        Array.Copy(part, 0, result, pos, part.Length);
        pos+=part.Length;
      }
      return result;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="startRow"></param>
    /// <param name="numberOfRows"></param>
    /// <returns></returns>
    public static T[] Join<T>(this T[][] array, int startRow, int numberOfRows) {
      if (startRow<0&&numberOfRows<0&&startRow+numberOfRows>array.Length)
        throw new ArgumentOutOfRangeException();
      numberOfRows+=startRow;

      var length = 0;
      T[] part;
      for (var i = startRow; i<numberOfRows; i++) {
        if ((part=array[i])!=null)
          length+=part.Length;
      }

      var result = new T[length];
      DoJoinTo(array, result, startRow, numberOfRows);
      return result;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="outArray"></param>
    /// <param name="startRow"></param>
    /// <param name="toRow"></param>
    /// <returns></returns>
    private static void DoJoinTo<T>(this T[][] array, T[] outArray, int startRow, int toRow) {
      var pos = 0;
      for (; startRow<toRow; startRow++) {
        T[] part;
        if ((part=array[startRow])==null)
          continue;
        Array.Copy(part, 0, outArray, pos, part.Length);
        pos+=part.Length;
      }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="outArray"></param>
    /// <param name="startRow"></param>
    /// <param name="numberOfRows"></param>
    /// <returns></returns>
    public static void JoinTo<T>(this T[][] array, T[] outArray, int startRow, int numberOfRows) {
      if (startRow<0&&numberOfRows<0&&startRow+numberOfRows>array.Length)
        throw new ArgumentOutOfRangeException();
      DoJoinTo(array, outArray, startRow, startRow+numberOfRows);
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T[] Join<T>(this List<T[]> array) {
      // TODO: test it!
      var length = 0;
      foreach (var part in array)
        if (part!=null)
          length+=part.Length;

      var result = new T[length];

      var pos = 0;
      foreach (var part in array) {
        if (part==null)
          continue;
        Array.Copy(part, 0, result, pos, part.Length);
        pos+=part.Length;
      }
      return result;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T[] Join<T>(this IEnumerable<T[]> array) {
      return Join(array as T[][]??array.ToArray());
    }

    public static T[] SubArray<T>(this T[] data, int index, int length) {
      var result = new T[length];
      Array.Copy(data, index, result, 0, length);
      return result;
    }

    public static T[] SubArrayDeepClone<T>(this T[] data, int index, int length) {
      var arrCopy = new T[length];
      Array.Copy(data, index, arrCopy, 0, length);
      using (var ms = new MemoryStream()) {
        var bf = new BinaryFormatter();
        bf.Serialize(ms, arrCopy);
        ms.Position=0;
        return (T[])bf.Deserialize(ms);
      }
    }

    public static T[,] To2D<T>(this T[] thisArray, int width) {
      var height = thisArray.Length/width;
      if (width*height!=thisArray.Length)
        throw new ArgumentOutOfRangeException(nameof(width), @"Length of array is not n times of width.");

      var result = new T[width, height];
      for (int y = 0, i = 0; y<height; y++)
        for (var x = 0; x<width; x++, i++)
          result[x, y]=thisArray[i];
      return result;
    }

    public static T[][] ToJagged<T>(this T[] thisArray, int width) {
      var h = thisArray.Length/width;
      if (h*width!=thisArray.Length)
        throw new ArgumentOutOfRangeException(nameof(width), @"Length of array is not n times of width.");
      var result = new T[h][];
      for (int y = 0, i = 0; y<h; y++, i+=width)
        Array.Copy(thisArray, i, result[y]=new T[width], 0, width);
      return result;
    }

    public static T[][] ToJagged<T>(this T[] thisArray, int width, bool transposed) {
      return transposed ? ToJagged(thisArray, width) : ToJaggedTransposed(thisArray, width);
    }

    public static T[][] ToJaggedTransposed<T>(this T[] thisArray, int width) {
      var h = thisArray.Length/width;
      if (h*width!=thisArray.Length)
        throw new ArgumentOutOfRangeException(nameof(width), @"Length of array is not n times of width.");
      var result = new T[width][];
      for (var y = 0; y<width; y++) {
        var col = result[y];
        for (int j = 0, i = y; j<col.Length; j++, i+=width)
          col[j]=thisArray[i];
      }
      return result;
    }

    public static void Copy<T>(this T[][] thisArray, T[] dstArray) {
      var i = 0;
      foreach (var a in thisArray) {
        Array.Copy(a, 0, dstArray, i, a.Length);
        i+=a.Length;
      }
    }

    public static void Copy<T>(this T[][] thisArray, T[] dstArray, bool transposed) {
      if (!transposed) {
        Copy(thisArray, dstArray);
        return;
      }
      var w = thisArray.Length;
      for (var y = 0; y<w; y++) {
        var col = thisArray[y];
        for (int j = 0, i = y; j<col.Length; j++, i+=w)
          dstArray[i]=col[j];
      }
    }

    public static void Copy<T>(this T[][] thisArray, T[][] dstArray) {
      var i = 0;
      foreach (var a in thisArray) {
        Array.Copy(a, 0, dstArray, i, a.Length);
        i+=a.Length;
      }
    }

    public static void Copy<T>(this T[][] thisArray, T[][] dstArray, bool transposed) {
      if (transposed)
        CopyTransposed(thisArray, dstArray);
      else
        Copy(thisArray, dstArray);
    }

    public static void CopyTransposed<T>(this T[][] thisArray, T[][] dstArray) {
      for (var y = 0; y<thisArray.Length; y++) {
        var col = thisArray[y];
        for (var x = 0; x<col.Length; x++)
          dstArray[x][y]=col[x];
      }
    }

    public static bool EqualElements<T>(this T[] thisArray, T[] otherArray) {
      var n = thisArray.Length;
      if (n!=otherArray.Length)
        return false;
      for (var i = 0; i<n; i++)
        if (!Objectt.Equals(thisArray[i], otherArray[i]))
          return false;
      return true;
    }

    public static bool EqualElements(this int[] thisArray, int[] otherArray) {
      var n = thisArray.Length;
      if (n!=otherArray.Length)
        return false;
      for (var i = 0; i<n; i++)
        if (thisArray[i]!=otherArray[i])
          return false;
      return true;
    }

    public static bool EqualElements(this double[] thisArray, double[] otherArray) {
      var n = thisArray.Length;
      if (n!=otherArray.Length)
        return false;
      for (var i = 0; i<n; i++)
        if (thisArray[i]!=otherArray[i])
          return false;
      return true;
    }

    public static bool EqualElements(this float[] thisArray, float[] otherArray) {
      var n = thisArray.Length;
      if (n!=otherArray.Length)
        return false;
      for (var i = 0; i<n; i++)
        if (thisArray[i]!=otherArray[i])
          return false;
      return true;
    }

    public static int BestIndex<T>(this IEnumerable<T> enumerable, Func<T, double> calculateError) {
      using (var enu = enumerable.GetEnumerator()) {
        if (!enu.MoveNext())
          throw new ArgumentException("The enumerable should not be empty.");

        var result = 0;
        var error = calculateError(enu.Current);
        var i = 1;
        while (enu.MoveNext()) {
          var curError = calculateError(enu.Current);
          if (curError<error) {
            error=curError;
            result=i;
          }
          i++;
        }
        return result;
      }
    }
  }
}