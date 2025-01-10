using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

namespace CSharpFftDemo;

[MinColumn, MaxColumn, MeanColumn, MedianColumn]
[MemoryDiagnoser]
[DisassemblyDiagnoser]
public class DotnetBenchmarks
{
    [Params(8, 9, 10, 11, 12)]
    public static int Log2FftSize { get; set; }

    private static readonly int size = 1 << Log2FftSize;

    private static readonly Complex[] xyManaged = new Complex[size];
    private static readonly Complex[] xyOutManaged = new Complex[size];

    private readonly FftNativeC.DoubleComplex[] xyNative = new FftNativeC.DoubleComplex[size];
    private readonly FftNativeC.DoubleComplex[] xyOutNative = new FftNativeC.DoubleComplex[size];

    private readonly FftNativeRust.DoubleComplex[] xyRust = new FftNativeRust.DoubleComplex[size];
    private readonly FftNativeRust.DoubleComplex[] xyOutRust = new FftNativeRust.DoubleComplex[size];

    private FftNativeRust.FftHandle? fftHandle;

    public static void Calculate()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("---- BENCHMARK.NET ----");
        Console.ForegroundColor = ConsoleColor.Gray;

        _ = BenchmarkRunner.Run<DotnetBenchmarks>();
    }

 

    [GlobalSetup]
    public void Setup()
    {
        int i;

        for (i = 0; i < size / 2; i++)
        {
            xyManaged[i] = new Complex(1.0, 0.0);
        }

        for (i = size / 2; i < size; i++)
        {
            xyManaged[i] = new Complex(-1.0, 0.0);
        }

        for (i = 0; i < size / 2; i++)
        {
            xyNative[i] = new FftNativeC.DoubleComplex(1.0f, 0.0f);
        }

        for (i = size / 2; i < size; i++)
        {
            xyNative[i] = new FftNativeC.DoubleComplex(-1.0f, 0.0f);
        }

        for (i = 0; i < size / 2; i++)
        {
            xyRust[i] = new FftNativeRust.DoubleComplex(1.0f, 0.0f);
        }

        for (i = size / 2; i < size; i++)
        {
            xyRust[i] = new FftNativeRust.DoubleComplex(-1.0f, 0.0f);
        }

        fftHandle = new FftNativeRust.FftHandle();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        fftHandle?.Dispose();
    }

    [Benchmark(Baseline = true)]
    public void Managed()
    {
        Fft.Calculate(Log2FftSize, xyManaged, xyOutManaged);
    }

    [Benchmark]
    public void RustDoubleComplex()
    {
        fftHandle?.Fft(xyRust, xyOutRust, 1 << Log2FftSize);
    }

    [Benchmark]
    public void CDoubleComplex()
    {
        FftNativeC.Fft(Log2FftSize, xyNative, xyOutNative);
    }
}
