using System;
using System.Net.PeerToPeer;
using System.Text;

namespace Util.Network {
  public class LocalPeer : IDisposable {
    private PeerNameRegistration _peerRegist;
    private PeerNameResolver _peerResolver = new PeerNameResolver();

    //private readonly TcpClient _client;
    //private readonly TcpListener _listener;

    public LocalPeer(string name, int port, Cloud cloud) {
      var peerName = new PeerName(name, PeerNameType.Secured);
      // _client = new TcpClient();
      //_listener = new TcpListener();

      _peerRegist=new PeerNameRegistration(peerName, port, cloud);
      _peerRegist.Start();
    }

    public bool Disposed { get; private set; }

    public void Dispose() {
      Dispose(true);
      // Use SupressFinalize in case a subclass
      // of this type implements a finalizer.
      GC.SuppressFinalize(this);
    }

    public void Stop() {
      _peerRegist.Stop();
    }

    public void Send(PeerName peer, byte[] data) {
      var results = _peerResolver.Resolve(peer);

      var count = 1;
      foreach (var record in results) {
        Console.WriteLine("Record #{0} results...", count);
        Console.Write("Comment:");

        if (record.Comment!=null)
          Console.Write(record.Comment);

        Console.WriteLine();
        Console.Write("Data:");

        if (record.Data!=null)
          Console.Write(Encoding.ASCII.GetString(record.Data));

        Console.WriteLine();
        Console.WriteLine("Endpoints:");

        foreach (var endpoint in record.EndPointCollection) {
          Console.WriteLine("\t Endpoint:{0}", endpoint);
          Console.WriteLine();
        }

        count++;
      }
    }

    protected virtual void Dispose(bool disposing) {
      // If you need thread safety, use a lock around these  
      // operations, as well as in your methods that use the resource.
      if (Disposed)
        return;

      if (disposing)
        _peerRegist.Dispose();


      // Indicate that the instance has been disposed.
      Disposed=true;
      _peerRegist=null;
      _peerResolver=null;
    }

    ~LocalPeer() {
      Dispose(false);
    }
  }
}