using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Util.IO.Serialization {
  public class Serializer<T> : ISerializer<T> {
    private readonly BinaryFormatter _bf = new BinaryFormatter();

    public byte[] Serialize(T obj) {
      using (var mem = new MemoryStream()) {
        _bf.Serialize(mem, obj);
        return mem.ToArray();
      }
    }

    public T Deserialize(byte[] data) {
      using (var mem = new MemoryStream(data))
        return (T)_bf.Deserialize(mem);
    }
  }
}