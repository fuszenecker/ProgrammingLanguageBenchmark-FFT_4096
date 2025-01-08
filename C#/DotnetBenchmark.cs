using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

using static CSharpFftDemo.GlobalResourceManager;

namespace CSharpFftDemo;

[MinColumn, MaxColumn, MeanColumn, MedianColumn]
[MemoryDiagnoser]
public class DotnetBenchmark
{
    private static readonly int size = 1 << Params.Log2FftSize;
    private static readonly Complex[] xyManaged = new Complex[size];
    private static readonly Complex[] xyOutManaged = new Complex[size];

    private readonly FftNative.DoubleComplex[] xyNative = new FftNative.DoubleComplex[size];
    private readonly FftNative.DoubleComplex[] xyOutNative = new FftNative.DoubleComplex[size];

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
    }

    [Benchmark]
    public void Managed()
    {
        Fft.Calculate(Params.Log2FftSize, xyManaged, xyOutManaged);
    }

    [Benchmark(Baseline = true)]
    public void Native()
    {
        FftNative.Fft(Params.Log2FftSize, xyNative, xyOutNative);
    }
}