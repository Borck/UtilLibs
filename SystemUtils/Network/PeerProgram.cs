using System;
using System.Net.PeerToPeer;
using System.Text;

namespace Util.Network {
  // PNRP Registration Sample
  internal class PeerProgram {
    private static void Main(string[] args) {
      // Creates a secure (not spoofable) PeerName
      var peerName = new PeerName("MikesWebServer", PeerNameType.Secured);
      var pnReg = new PeerNameRegistration {
        PeerName=peerName,
        Port=80,
        //OPTIONAL
        //The properties set below are optional.  You can register a PeerName without setting these properties
        Comment="up to 39 unicode char comment",
        Data=Encoding.UTF8.GetBytes("A data blob associated with the name")
      };

      /*
       * OPTIONAL
       *The properties below are also optional, but will not be set (ie. are commented out) for this example
       *pnReg.IPEndPointCollection = // a list of all {IPv4/v6 address, port} pairs to associate with the peername
       *pnReg.Cloud = //the scope in which the name should be registered (local subnet, internet, etc)
      */

      //Starting the registration means the name is published for others to resolve
      pnReg.Start();
      Console.WriteLine("Registration of Peer Name: {0} complete.", peerName);
      Console.WriteLine();
      Console.WriteLine("Press any key to stop the registration and close the program");
      Console.ReadKey();

      pnReg.Stop();
    }
  }
}