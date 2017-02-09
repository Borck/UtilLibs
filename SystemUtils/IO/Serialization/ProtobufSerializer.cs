using ProtoBuf;
using System.IO;

namespace Util.IO.Serialization {
  /// <summary>
  ///   Getting started: https://code.google.com/p/protobuf-net/wiki/GettingStarted
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ProtobufSerializer<T> : ISerializer<T> {
    public byte[] Serialize(T obj) {
      using (var mem = new MemoryStream()) {
        Serializer.Serialize(mem, obj);
        return mem.ToArray();
      }
    }

    public T Deserialize(byte[] data) {
      using (var mem = new MemoryStream(data))
        return Serializer.Deserialize<T>(mem);
    }
  }
}