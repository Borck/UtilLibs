namespace System.OpenCL {
  internal class OclKernelWrapper {
    /*internal static readonly Context _context;
    internal static readonly Device _device;


    internal static void CheckErr(ErrorCode err, string name) {
      if (err != ErrorCode.Success) {
        Console.WriteLine("ERROR: " + name + " (" + err + ")");
      }
    }
    internal static void ContextNotify(string errInfo, byte[] data, IntPtr cb, IntPtr userData) {
      Console.WriteLine("OpenCL Notification: " + errInfo);
    }
    
    static OclKernelWrapper()
    {
      ErrorCode error;
      Platform[] platforms = Cl.GetPlatformIDs(out error);
      List<Device> devicesList = new List<Device>();

      CheckErr(error, "Cl.GetPlatformIDs");

      foreach (Platform platform in platforms) {
        string platformName = Cl.GetPlatformInfo(platform, PlatformInfo.Name, out error).ToString();
        Console.WriteLine("Platform: " + platformName);
        CheckErr(error, "Cl.GetPlatformInfo");
        //We will be looking only for GPU devices
        foreach (Device device in Cl.GetDeviceIDs(platform, DeviceType.Gpu, out error)) {
          CheckErr(error, "Cl.GetDeviceIDs");
          Console.WriteLine("Device: " + device.ToString());
          devicesList.Add(device);
        }
      }

      if (devicesList.Count <= 0) {
        Console.WriteLine("No devices found.");
        return;
      }

      _device = devicesList[0];

      if (Cl.GetDeviceInfo(_device, DeviceInfo.ImageSupport,
                out error).CastTo<Bool>() == Bool.False) {
        Console.WriteLine("No image support.");
        return;
      }
      _context = Cl.CreateContext(null, 1, new[] { _device }, ContextNotify,
        IntPtr.Zero, out error);    //Second parameter is amount of devices
      CheckErr(error, "Cl.CreateContext");

    }*/
  }
}