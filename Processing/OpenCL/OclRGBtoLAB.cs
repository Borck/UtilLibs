namespace System.OpenCL {
  public class OclRgbToLab {
    // Schwellenwert:
    private const double T = 0.008856;

    private const byte MIN_RGB = 0;
    private const byte MAX_RGB = 255;


    private const double T1 = 0.008856;
    private const double T2 = 0.206893;

    /*private readonly Context _context;
    private readonly Device _device;
    
    private readonly clRGBtoLAB _kernel;

    private static Program RgbToLabProgram;


    public OclRgbToLab()
    {
      _context = OclKernelWrapper._context;
      _device = OclKernelWrapper._device;
      _kernel = new clRGBtoLAB(_context);

      ErrorCode error;



      RgbToLabProgram = Cl.CreateProgramWithSource(_context, 1, new[] { _kernel.getKernelSource() }, null, out error);
      OclKernelWrapper.CheckErr(error, "Cl.CreateProgramWithSource");


      //Compile kernel source
      error = Cl.BuildProgram(RgbToLabProgram, 1, new[] { _device }, string.Empty, null, IntPtr.Zero);
      OclKernelWrapper.CheckErr(error, "Cl.BuildProgram");
      //Check for any compilation errors
      if (Cl.GetProgramBuildInfo(RgbToLabProgram, _device, ProgramBuildInfo.Status, out error).CastTo<BuildStatus>()
          != BuildStatus.Success) {
            OclKernelWrapper.CheckErr(error, "Cl.GetProgramBuildInfo");
        Console.WriteLine("Cl.GetProgramBuildInfo != Success");
        Console.WriteLine(Cl.GetProgramBuildInfo(RgbToLabProgram, _device, ProgramBuildInfo.Log, out error));
        return;
      }
      //Create the required kernel (entry function)
      //_kernel = Cl.CreateKernel(RgbToLabProgram, "imagingTest", out error);
      OclKernelWrapper.CheckErr(error, "Cl.CreateKernel");

      int intPtrSize = 0;
      intPtrSize = Marshal.SizeOf(typeof(IntPtr));
    }*/


    private static readonly double[] Xn = {0.950456, 1, 1.088754};

    private static readonly double[,] CRx = {
      {0.412453, 0.357580, 0.180423},
      {0.212671, 0.715160, 0.072169},
      {0.019334, 0.119193, 0.950227}
    };

    private static readonly double[,] CXr = {
      {3.240479, -1.537150, -0.498535},
      {-0.969256, 1.875992, 0.041556},
      {0.055648, -0.204043, 1.057311}
    };


    public static void RgbToLab(byte[] input, ref byte[] output) {
      if (input == null || output == null)
        throw new NullReferenceException("");

      if (input.Length != output.Length)
        throw new ArgumentException("");


      var length = input.Length;

      var XYZ = new double[3];
      for (var i = 0; i < length; i += 4) {
        double[] rgb = {
          input[i + 2]/255f,
          input[i + 1]/255f,
          input[i]/255f
        };


        // RGB -> XYZ. Dabei selben Referenzweißpunkt D65 verwenden:
        var xyz = new double[3];


        xyz[0] = CRx[0, 0]*rgb[0] + CRx[0, 1]*rgb[1] + CRx[0, 2]*rgb[2];
        xyz[1] = CRx[1, 0]*rgb[0] + CRx[1, 1]*rgb[1] + CRx[1, 2]*rgb[2];
        xyz[2] = CRx[2, 0]*rgb[0] + CRx[2, 1]*rgb[1] + CRx[2, 2]*rgb[2];

        // Auf Referenzpunkt normieren:
        XYZ[0] = xyz[0]/Xn[0];
        XYZ[1] = xyz[1]/Xn[1];
        XYZ[2] = xyz[2]/Xn[2];


        var xt = XYZ[0] > T;
        var yt = XYZ[1] > T;
        var zt = XYZ[2] > T;
        var y3 = Math.Pow(XYZ[1], 1d/3d);

        // Nichtlinear Projektion von XYZ -> Lab:
        var fX = xt ? Math.Pow(XYZ[0], 1d/3d) : 7.787*XYZ[0] + 16d/116d;
        var fY = yt ? y3 : 7.787*XYZ[1] + 16d/116d;
        var fZ = zt ? Math.Pow(XYZ[2], 1d/3d) : 7.787*XYZ[1] + 16d/116d;


        var lab = new double[3];

        lab[0] = yt ? 116d*y3 - 16d : 903.3*XYZ[1];
        lab[1] = 500d*(fX - fY);
        lab[2] = 200d*(fY - fZ);

        // RGB in [0,1] -> RGB in [0,255]. Dabei auch die Wert in dieses
        // Intervall zwingen:
        output[i + 2] = (lab[0] < 0) ? MIN_RGB : (lab[0] > 100 ? MAX_RGB : (byte) (lab[0]*2.55));
        output[i + 1] = (lab[1] < -150) ? MIN_RGB : (lab[1] > 100 ? MAX_RGB : (byte) ((lab[1] + 150)*1.02));
        output[i] = (lab[2] < -100) ? MIN_RGB : (lab[2] > 150 ? MAX_RGB : (byte) ((lab[2] + 100)*1.02));
        output[i + 3] = input[i + 3];
      }
    }

    public static void LabToRgb(byte[] input, ref byte[] output) {
      if (input == null || output == null)
        throw new NullReferenceException("");

      if (input.Length != output.Length)
        throw new ArgumentException("");

      //-----------------------------------------------------------------
      var xyz = new double[3];

      var length = input.Length;
      for (var i = 0; i < length; i += 4) {
        double[] lab = {
          input[i + 2]/2.55f,
          input[i + 1]/1.02f - 150,
          input[i]/1.02f - 100
        };


        // Y berechnen:
        var fY = Math.Pow((lab[0] + 16d)/116d, 3);
        var yt = fY > T1;
        fY = yt ? fY : lab[0]/903.3;
        xyz[1] = fY;

        // fY leicht modifizieren für die weiteren Berechnungen:
        fY = yt ? Math.Pow(fY, 1d/3d) : 7.787*fY + 16d/116d;

        // X berechnen:
        var fX = lab[1]/500d + fY;
        xyz[0] = fX > T2 ? Math.Pow(fX, 3) : (fX - 16d/116d)/7.787;

        // Z berechnen:
        var fZ = fY - lab[2]/200d;
        xyz[2] = fZ > T2 ? Math.Pow(fZ, 3) : (fZ - 16d/116d)/7.787;

        // Auf Referenzweiß D65 normalisieren:
        for (var j = 0; j < xyz.Length; j++)
          xyz[j] *= Xn[j];

        // XYZ -> RGB. Dabei selben Referenzweißpunkt D65 verwenden:
        var rgb = new double[3];

        rgb[0] = CXr[0, 0]*xyz[0] + CXr[0, 1]*xyz[1] + CXr[0, 2]*xyz[2];
        rgb[1] = CXr[1, 0]*xyz[0] + CXr[1, 1]*xyz[1] + CXr[1, 2]*xyz[2];
        rgb[2] = CXr[2, 0]*xyz[0] + CXr[2, 1]*xyz[1] + CXr[2, 2]*xyz[2];

        // RGB in [0,1] -> RGB in [0,255]. Dabei auch die Wert in dieses
        // Intervall zwingen:
        output[i + 2] = (rgb[0] < 0) ? (byte) 0 : (rgb[0] > 1 ? (byte) 255 : (byte) (rgb[0]*255));
        output[i + 1] = (rgb[1] < 0) ? (byte) 0 : (rgb[1] > 1 ? (byte) 255 : (byte) (rgb[1]*255));
        output[i] = (rgb[2] < 0) ? (byte) 0 : (rgb[2] > 1 ? (byte) 255 : (byte) (rgb[2]*255));
        output[i + 3] = input[i + 3];
      }
    }


    /*public byte[] ToLab(byte[] inputBytes) {
        ErrorCode error;

        //Image's RGBA data converted to an unmanaged[] array
        //OpenCL memory buffer that will keep our image's byte[] data.
        Mem inputImage2DBuffer;
        ImageFormat clImageFormat = new ImageFormat(ChannelOrder.RGBA, ChannelType.Unsigned_Int8);
        int inputImgWidth, inputImgHeight;

        int inputImgBytesSize;
        int inputImgStride;
        //Try loading the input image
        
          //Allocate OpenCL image memory buffer
          inputImage2DBuffer = Cl.CreateImage2D(_context, MemFlags.CopyHostPtr | MemFlags.ReadOnly, clImageFormat,
                                              (IntPtr)bitmapData.Width, (IntPtr)bitmapData.Height,
                                              (IntPtr)0, inputBytes, out error);
          OclKernelWrapper.CheckErr(error, "Cl.CreateImage2D input");
        
        //Unmanaged output image's raw RGBA byte[] array
        byte[] outputByteArray = new byte[inputImgBytesSize];
        //Allocate OpenCL image memory buffer
        Mem outputImage2DBuffer = Cl.CreateImage2D(_context, MemFlags.CopyHostPtr |
            MemFlags.WriteOnly, clImageFormat, (IntPtr)inputImgWidth,
            (IntPtr)inputImgHeight, (IntPtr)0, outputByteArray, out error);
        OclKernelWrapper.CheckErr(error, "Cl.CreateImage2D output");
        //Pass the memory buffers to our kernel function
        error = Cl.SetKernelArg(_kernel, 0, (IntPtr)intPtrSize, inputImage2DBuffer);
        error |= Cl.SetKernelArg(_kernel, 1, (IntPtr)intPtrSize, outputImage2DBuffer);
        OclKernelWrapper.CheckErr(error, "Cl.SetKernelArg");

        //Create a command queue, where all of the commands for execution will be added
        CommandQueue cmdQueue = Cl.CreateCommandQueue(_context, _device, (CommandQueueProperties)0, out error);
        OclKernelWrapper.CheckErr(error, "Cl.CreateCommandQueue");
        Event clevent;
        //Copy input image from the host to the GPU.
        IntPtr[] originPtr = { (IntPtr)0, (IntPtr)0, (IntPtr)0 };    //x, y, z
        IntPtr[] regionPtr = { (IntPtr)inputImgWidth, (IntPtr)inputImgHeight, (IntPtr)1 };    //x, y, z
        IntPtr[] workGroupSizePtr = { (IntPtr)inputImgWidth, (IntPtr)inputImgHeight, (IntPtr)1 };
        error = Cl.EnqueueWriteImage(cmdQueue, inputImage2DBuffer, Bool.True,
           originPtr, regionPtr, (IntPtr)0, (IntPtr)0, inputBytes, 0, null, out clevent);





        OclKernelWrapper.CheckErr(error, "Cl.EnqueueWriteImage");
        //Execute our kernel (OpenCL code)
        error = Cl.EnqueueNDRangeKernel(cmdQueue, _kernel, 2, null, workGroupSizePtr, null, 0, null, out clevent);
        OclKernelWrapper.CheckErr(error, "Cl.EnqueueNDRangeKernel");
        //Wait for completion of all calculations on the GPU.
        error = Cl.Finish(cmdQueue);
        OclKernelWrapper.CheckErr(error, "Cl.Finish");
        //Read the processed image from GPU to raw RGBA data byte[] array
        error = Cl.EnqueueReadImage(cmdQueue, outputImage2DBuffer, Bool.True, originPtr, regionPtr,
                                    (IntPtr)0, (IntPtr)0, outputByteArray, 0, null, out clevent);

        OclKernelWrapper.CheckErr(error, "Cl.clEnqueueReadImage");
        //Clean up memory
        Cl.ReleaseKernel(_kernel);
        Cl.ReleaseCommandQueue(cmdQueue);

        Cl.ReleaseMemObject(inputImage2DBuffer);
        Cl.ReleaseMemObject(outputImage2DBuffer);
        //Get a pointer to our unmanaged output byte[] array
        GCHandle pinnedOutputArray = GCHandle.Alloc(outputByteArray, GCHandleType.Pinned);
        IntPtr outputBmpPointer = pinnedOutputArray.AddrOfPinnedObject();
        //Create a new bitmap with processed data and save it to a file.
        Bitmap outputBitmap = new Bitmap(inputImgWidth, inputImgHeight,
              inputImgStride, PixelFormat.Format32bppArgb, outputBmpPointer);

        outputBitmap.Save(outputImagePath, System.Drawing.Imaging.ImageFormat.Png);
        pinnedOutputArray.Free();
    }*/
  }
}