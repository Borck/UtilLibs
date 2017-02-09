using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;

namespace Util.Numerics {
  [TestClass]
  public class MathhTest {

    [TestMethod]
    public void UpperPowerOf2UintTest() {
      Assert.AreEqual((uint)0, Mathh.UpperPow2(0));
      Assert.AreEqual((uint)1, Mathh.UpperPow2(1));
      Assert.AreEqual((uint)2, Mathh.UpperPow2(2));
      Assert.AreEqual((uint)4, Mathh.UpperPow2(3));
      Assert.AreEqual((uint)4, Mathh.UpperPow2(4));
      Assert.AreEqual((uint)8, Mathh.UpperPow2(5));
      Assert.AreEqual((uint)8, Mathh.UpperPow2(6));
      Assert.AreEqual((uint)8, Mathh.UpperPow2(7));
      Assert.AreEqual((uint)8, Mathh.UpperPow2(8));
      Assert.AreEqual((uint)16, Mathh.UpperPow2(9));
      Assert.AreEqual((uint)16, Mathh.UpperPow2(10));
      Assert.AreEqual((uint)16, Mathh.UpperPow2(11));
      Assert.AreEqual((uint)16, Mathh.UpperPow2(12));
      Assert.AreEqual((uint)16, Mathh.UpperPow2(13));
      Assert.AreEqual((uint)16, Mathh.UpperPow2(14));
      Assert.AreEqual((uint)16, Mathh.UpperPow2(15));
      Assert.AreEqual((uint)16, Mathh.UpperPow2(16));
      Assert.AreEqual((uint)32, Mathh.UpperPow2(17));
    }

    [TestMethod]
    public void UpperPowerOf2UlongTest() {
      Assert.AreEqual((ulong)0, Mathh.UpperPow2(0));
      Assert.AreEqual((ulong)1, Mathh.UpperPow2(1));
      Assert.AreEqual((ulong)2, Mathh.UpperPow2(2));
      Assert.AreEqual((ulong)4, Mathh.UpperPow2(3));
      Assert.AreEqual((ulong)4, Mathh.UpperPow2(4));
      Assert.AreEqual((ulong)8, Mathh.UpperPow2(5));
      Assert.AreEqual((ulong)8, Mathh.UpperPow2(6));
      Assert.AreEqual((ulong)8, Mathh.UpperPow2(7));
      Assert.AreEqual((ulong)8, Mathh.UpperPow2(8));
      Assert.AreEqual((ulong)16, Mathh.UpperPow2(9));
      Assert.AreEqual((ulong)16, Mathh.UpperPow2(10));
      Assert.AreEqual((ulong)16, Mathh.UpperPow2(11));
      Assert.AreEqual((ulong)16, Mathh.UpperPow2(12));
      Assert.AreEqual((ulong)16, Mathh.UpperPow2(13));
      Assert.AreEqual((ulong)16, Mathh.UpperPow2(14));
      Assert.AreEqual((ulong)16, Mathh.UpperPow2(15));
      Assert.AreEqual((ulong)16, Mathh.UpperPow2(16));
      Assert.AreEqual((ulong)32, Mathh.UpperPow2(17));
    }

    /// <summary>
    /// phase interpolation
    /// </summary>
    /// <param name="timesOfPi1"></param>
    /// <param name="timesOfPi2"></param>
    /// <param name="a"></param>
    private void DoPhaseIpol(double timesOfPi1, double timesOfPi2, double a) {
      var phi1 = timesOfPi1*Math.PI;
      var phi2 = timesOfPi2*Math.PI;
      var phiR = Mathh.InterpolatePhase(phi1, phi2, a);
      var angle1 = Mathh.RadToDeg(phi1);
      var angle2 = Mathh.RadToDeg(phi2);
      var angleR = Mathh.RadToDeg(phiR);
      Console.WriteLine($"{angle1}->{angle2}({a})={angleR}");
    }

    [TestMethod]
    public void PhaseInterpolation() {
      DoPhaseIpol(1.0/2, -1.0/3, .1);


      //Assert.AreEqual();
    }

    [TestMethod]
    public void ComplexPhaseVerification() {
      var c = new Complex(-1, -.00);
      var phi = Mathh.RadToDeg(c.Phase);
      Console.WriteLine(phi);
    }

    [TestMethod]
    public void Atan2Verification() {
      var phi = Math.Atan2(.5, 1);
      var angle = Mathh.RadToDeg(phi);
    }
  }

}