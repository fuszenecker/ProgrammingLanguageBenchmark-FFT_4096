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

    private readonly FftNative.DoubleComplex[] xyNative = new FftNative.DoubleComplex[size];
    private readonly FftNative.DoubleComplex[] xyOutNative = new FftNative.DoubleComplex[size];

    private readonly FftRust.FloatComplex[] xyRust = new FftRust.FloatComplex[size];
    private readonly FftRust.FloatComplex[] xyOutRust = new FftRust.FloatComplex[size];

    private FftRust.FftHandle fftHandle = null;

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
            xyNative[i] = new FftNative.DoubleComplex(1.0f, 0.0f);
        }

        for (i = size / 2; i < size; i++)
        {
            xyNative[i] = new FftNative.DoubleComplex(-1.0f, 0.0f);
        }

        for (i = 0; i < size / 2; i++)
        {
            xyRust[i] = new FftRust.FloatComplex(1.0f, 0.0f);
        }

        for (i = size / 2; i < size; i++)
        {
            xyRust[i] = new FftRust.FloatComplex(-1.0f, 0.0f);
        }
    }

    [GlobalSetup]
    public void Setup()
    {
	fftHandle = new FftRust.FftHandle();
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
    public void Rust_FloatComplex()
    {
        fftHandle.Fft(xyRust, xyOutRust, size);
    }

    [Benchmark(Baseline = true)]
    public void C_DoubleComplex()
    {
        FftNative.Fft(Params.Log2FftSize, xyNative, xyOutNative);
    }
}
