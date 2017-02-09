using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace Util.IO.Serialization {
  public class ZlibCompression<T> : ISerializer<T> {
    private readonly BinaryFormatter _bf = new BinaryFormatter();

    public byte[] Serialize(T obj) {
      MemoryStream mem = null;
      byte[] result;
      try {
        mem=new MemoryStream();
        using (var dfs = new DeflateStream(mem, CompressionMode.Compress, true))
          _bf.Serialize(dfs, obj);
      } finally {
        if (mem!=null) {
          result=mem.ToArray();
          mem.Dispose();
        } else {
          result=null;
        }
      }
      return result;
    }

    public T Deserialize(byte[] data) {
      MemoryStream mem = null;
      T result;
      try {
        mem=new MemoryStream(data);
        using (var dfs = new DeflateStream(mem, CompressionMode.Decompress, true))
          result=(T)_bf.Deserialize(dfs);
      } finally {
        mem?.Dispose();
      }
      return result;
    }
  }
}