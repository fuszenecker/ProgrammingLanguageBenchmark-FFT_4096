using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

using static CSharpFftDemo.GlobalResourceManager;

namespace CSharpFftDemo;

[MinColumn, MaxColumn, MeanColumn, MedianColumn]
[MemoryDiagnoser]
[DisassemblyDiagnoser]
public class DotnetBenchmark
{
    private static readonly int size = 1 << Params.Log2FftSize;
    private static readonly Complex[] xyManaged = new Complex[size];
    private static readonly Complex[] xyOutManaged = new Complex[size];

    private readonly NativeC.DoubleComplex[] xyNative = new NativeC.DoubleComplex[size];
    private readonly NativeC.DoubleComplex[] xyOutNative = new NativeC.DoubleComplex[size];

    private readonly NativeRust.DoubleComplex[] xyRust = new NativeRust.DoubleComplex[size];
    private readonly NativeRust.DoubleComplex[] xyOutRust = new NativeRust.DoubleComplex[size];

    private NativeRust.FftHandle fftHandle = null;

    public static void Calculate()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(GetStringResource("BenchmarkDotNetText"));
        Console.ForegroundColor = ConsoleColor.Gray;

        _ = BenchmarkRunner.Run<DotnetBenchmark>();
    }

    public DotnetBenchmark()
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
            xyNative[i] = new NativeC.DoubleComplex(1.0f, 0.0f);
        }

        for (i = size / 2; i < size; i++)
        {
            xyNative[i] = new NativeC.DoubleComplex(-1.0f, 0.0f);
        }

        for (i = 0; i < size / 2; i++)
        {
            xyRust[i] = new NativeRust.DoubleComplex(1.0f, 0.0f);
        }

        for (i = size / 2; i < size; i++)
        {
            xyRust[i] = new NativeRust.DoubleComplex(-1.0f, 0.0f);
        }
    }

    [GlobalSetup]
    public void Setup()
    {
	fftHandle = new NativeRust.FftHandle();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
	fftHandle.Dispose();
    }

    [Benchmark]
    public void Managed()
    {
        Fft.Calculate(Params.Log2FftSize, xyManaged, xyOutManaged);
    }

    [Benchmark]
    public void Rust_DoubleComplex()
    {
        fftHandle.Fft(xyRust, xyOutRust, size);
    }

    [Benchmark(Baseline = true)]
    public void C_DoubleComplex()
    {
        NativeC.Fft(Params.Log2FftSize, xyNative, xyOutNative);
    }
}
