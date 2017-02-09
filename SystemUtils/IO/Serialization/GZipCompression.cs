using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace Util.IO.Serialization {
  public class GZipCompression<T> : ISerializer<T> {
    private readonly BinaryFormatter _bf = new BinaryFormatter();

    public byte[] Serialize(T obj) {
      MemoryStream mem = null;
      byte[] result;
      try {
        mem=new MemoryStream();
        using (var gzip = new GZipStream(mem, CompressionMode.Compress, true)) {
          _bf.Serialize(gzip, obj);
        }
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
        using (var gzip = new GZipStream(mem, CompressionMode.Decompress, true))
          result=(T)_bf.Deserialize(gzip);
      } finally {
        mem?.Dispose();
      }
      return result;
    }
  }
}