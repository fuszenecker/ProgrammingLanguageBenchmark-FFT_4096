using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using static CSharpFftDemo.GlobalResourceManager;

namespace CSharpFftDemo;

internal static partial class NativeRust
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct FloatComplex(float real, float imaginary)
    {
        [FieldOffset(0)]
        public float Real = real;

        [FieldOffset(4)]
        public float Imaginary = imaginary;

        public override readonly string ToString()
        {
            return $"(Re: {Real}, Im: {Imaginary})";
        }
    }

    public partial class FftHandle : SafeHandle
    {
        public FftHandle() : base(0, true)
        {
	    Console.WriteLine("Creating new Rust FFT handle.");
            SetHandle(RustNew());
	    Console.WriteLine("Rust FFT handle: 0x{0:X}", handle);
        }

        protected override bool ReleaseHandle()
        {
	    Console.WriteLine("Disposing Rust FFT handle.");
	    Free(handle);
	    Console.WriteLine("Disposed Rust FFT handle.");
            return true;
        }

        public override bool IsInvalid
	{
            get
            {
                return handle == IntPtr.Zero;
	    }
	}

        public void Fft(FloatComplex[] xy_out, FloatComplex[] xy_in, int size)
	{
            Fft(handle, xy_out, xy_in, size);
	}

        [LibraryImport(".//libfftr.so", EntryPoint = "new")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.ApplicationDirectory)]
        internal static partial IntPtr RustNew();

        [LibraryImport("./libfftr.so", EntryPoint = "fft")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.ApplicationDirectory)]
        internal static partial void Fft(IntPtr self_ptr, FloatComplex[] xy_out, FloatComplex[] xy_in, int length);

	[LibraryImport("./libfftr.so", EntryPoint = "free")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.ApplicationDirectory)]
        internal static partial void Free(IntPtr self_ptr);
    }

    public static double Calculate(int log2FftSize, int fftRepeat)
    {
        int i;
        int size = 1 << log2FftSize;

        FloatComplex[] xy = new FloatComplex[size];
        FloatComplex[] xy_out = new FloatComplex[xy.Length];

        for (i = 0; i < size / 2; i++)
        {
            xy[i] = new FloatComplex(1.0f, 0.0f);
        }

        for (i = size / 2; i < size; i++)
        {
            xy[i] = new FloatComplex(-1.0f, 0.0f);
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

	try
	{
            using FftHandle fftHandle = new FftHandle();
        
	    for (i = 0; i < fftRepeat; i++)
            {
                fftHandle.Fft(xy_out, xy, size);
            }
	}
	catch (Exception ex)
	{
	    Console.WriteLine($"Exception: {ex.Message}");
	    return 0;
	}

        stopwatch.Stop();

        Console.WriteLine($"Total ({fftRepeat}): {stopwatch.ElapsedMilliseconds}");

        float tpp = stopwatch.ElapsedMilliseconds / (float)fftRepeat;

        Console.WriteLine($"{fftRepeat} piece(s) of {1 << log2FftSize} pt FFT;  {tpp} ms/piece\n");

        for (i = 0; i < 6; i++)
        {
            Console.WriteLine(GetStringResource("ZeroTabOne")!, i, xy_out[i]);
        }

        return tpp;
    }
}
