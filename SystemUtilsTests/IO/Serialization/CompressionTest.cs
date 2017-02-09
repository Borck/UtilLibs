using Microsoft.VisualStudio.TestTools.UnitTesting;
using Util.IO.Serialization;
using Util.Numerics;

namespace System {
  [TestClass]
  public class CompressionTest {
    private static void CheckSame(params int[][] dts) {
      if (dts.Length<=1)
        return;
      var d1 = dts[0];
      for (var i = 1; i<dts.Length; i++)
        if (d1.GetSad(dts[i])>0)
          Assert.Fail();
    }


    [TestMethod]
    public void CompressionProtobuf() {
      var data = RandInt(32);
      var comp = new ProtobufSerializer<int[]>();
      var cdata = comp.Serialize(data);
      var ddata = comp.Deserialize(cdata);

      CheckSame(data, ddata);
      var compression = (double)cdata.Length/(sizeof(int)*ddata.Length);
    }

    private static void AssertSame(int[] a, int[] b) {
      for (var i = 0; i<a.Length; i++)
        if (a[i]!=b[i])
          Assert.Fail();
    }

    private static int[] RandInt(int lenght, int minValue, int maxValue) {
      var rand = new Random((int)DateTime.Now.Ticks);
      var result = new int[lenght];
      for (var i = 0; i<result.Length; i++)
        result[i]=rand.Next(minValue, maxValue);
      return result;
    }

    private static int[] RandInt(int lenght) {
      var rand = new Random((int)DateTime.Now.Ticks);
      var result = new int[lenght];
      for (var i = 0; i<result.Length; i++)
        result[i]=rand.Next();
      return result;
    }

    private static double[] RandDouble(int lenght) {
      var rand = new Random((int)DateTime.Now.Ticks);
      var result = new double[lenght];
      for (var i = 0; i<result.Length; i++)
        result[i]=(double)rand.Next(1, 10000)/rand.Next(1, 10000);
      return result;
    }

    private static double[] RandDouble(int lenght, int step) {
      var rand = new Random((int)DateTime.Now.Ticks);
      var result = new double[lenght];
      for (var i = 0; i<result.Length; i++)
        result[i]=i%step==0 ? (double)rand.Next(1, 10000)/rand.Next(1, 10000) : 0;
      return result;
    }

    [TestMethod]
    public void CompressionCompare() {
      var data = RandInt(16000, 0, 2047);
      var ser = new Serializer<int[]>();
      var c1 = ser.Serialize(data);

      var gzipInt = new GZipCompression<int[]>();
      var c2 = gzipInt.Serialize(data);

      var proto = new ProtobufSerializer<int[]>();
      var gzipByte = new GZipCompression<byte[]>();
      var c3 = gzipByte.Serialize(proto.Serialize(data));

      var protogzip = new ProtoBufGZipCompression<int[]>();
      var c4 = protogzip.Serialize(data);

      var protozlib = new ProtoBufZlibCompression<int[]>();
      var c5 = protozlib.Serialize(data);

      var zlib = new ZlibCompression<int[]>();
      var c6 = zlib.Serialize(data);

      var d1 = ser.Deserialize(c1);
      var d2 = gzipInt.Deserialize(c2);
      var d3 = proto.Deserialize(gzipByte.Deserialize(c3));
      var d4 = protogzip.Deserialize(c4);
      var d5 = protozlib.Deserialize(c5);
      var d6 = zlib.Deserialize(c6);

      AssertSame(d1, d2);
      AssertSame(d3, d4);
      AssertSame(d5, d6);
      AssertSame(d1, d3);
      AssertSame(d1, d5);
    }

    [TestMethod]
    public void CompressIncreasingRedundanceTest() {
      const int compressions = 1024;
      var ser = new ProtoBufZlibCompression<double[]>();
      var result = new byte[compressions][];
      for (var i = 0; i<result.Length; i++)
        result[i]=ser.Serialize(RandDouble(8193, i+1));
    }
  }
}
