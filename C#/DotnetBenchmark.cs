using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

namespace CSharpFftDemo;

[MinColumn, MaxColumn, MeanColumn, MedianColumn]
[MemoryDiagnoser]
[DisassemblyDiagnoser]
public class DotnetBenchmark
{
    public static int Log2FftSize { get; set; }

    private int size;
    private Complex[] xyManaged = null!;
    private Complex[] xyOutManaged = null!;

    private FftNative.DoubleComplex[] xyNative = null!;
    private FftNative.DoubleComplex[] xyOutNative = null!;

    public static void Calculate(int log2FftSize)
    {
        Log2FftSize = log2FftSize;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("---- BENCHMARK.NET ----");
        Console.ForegroundColor = ConsoleColor.Gray;

        _ = BenchmarkRunner.Run<DotnetBenchmark>();
    }

    [GlobalSetup]
    public void Setup()
    {
        size = 1 << Log2FftSize;
        xyManaged = new Complex[size];
        xyOutManaged = new Complex[size];
        xyNative = new FftNative.DoubleComplex[size];
        xyOutNative = new FftNative.DoubleComplex[size];

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
    }

    [Benchmark]
    public void Managed()
    {
        Fft.Calculate(Log2FftSize, xyManaged, xyOutManaged);
    }

    [Benchmark(Baseline = true)]
    public void Native()
    {
        FftNative.Fft(Log2FftSize, xyNative, xyOutNative);
    }
}