using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;

namespace CSharpFftDemo;

[MinColumn, MaxColumn, MeanColumn, MedianColumn]
[MemoryDiagnoser]
[DisassemblyDiagnoser]
public class DotnetBenchmark : IDisposable
{
    private sealed class Config : ManualConfig
    {
        public Config()
        {
            AddLogger(ConsoleLogger.Default);
            AddColumnProvider(DefaultColumnProviders.Instance);
            WithOptions(ConfigOptions.DisableOptimizationsValidator);
        }
    }

    public static int Log2FftSize { get; set; }

    private int size;
    private Complex[] xyManaged = null!;
    private Complex[] xyOutManaged = null!;

    private FftNativeC.DoubleComplex[] xyNative = null!;
    private FftNativeC.DoubleComplex[] xyOutNative = null!;

    private FftNativeRust.DoubleComplex[] xyRust = null!;
    private FftNativeRust.DoubleComplex[] xyOutRust = null!;
    private FftNativeRust.FftHandle rustHandle = null!;

    public static void Calculate(int log2FftSize)
    {
        Log2FftSize = log2FftSize;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("---- BENCHMARK.NET ----");
        Console.ForegroundColor = ConsoleColor.Gray;

        _ = BenchmarkRunner.Run<DotnetBenchmark>(new Config());
    }

    [GlobalSetup]
    public void Setup()
    {
        size = 1 << Log2FftSize;
        xyManaged = new Complex[size];
        xyOutManaged = new Complex[size];
        xyNative = new FftNativeC.DoubleComplex[size];
        xyOutNative = new FftNativeC.DoubleComplex[size];
        xyRust = new FftNativeRust.DoubleComplex[size];
        xyOutRust = new FftNativeRust.DoubleComplex[size];
        rustHandle = new FftNativeRust.FftHandle();

        int i;

        for (i = 0; i < size / 2; i++)
        {
            xyManaged[i] = new Complex(1.0, 0.0);
            xyNative[i] = new FftNativeC.DoubleComplex(1.0f, 0.0f);
            xyRust[i] = new FftNativeRust.DoubleComplex(1.0f, 0.0f);
        }

        for (i = size / 2; i < size; i++)
        {
            xyManaged[i] = new Complex(-1.0, 0.0);
            xyNative[i] = new FftNativeC.DoubleComplex(-1.0f, 0.0f);
            xyRust[i] = new FftNativeRust.DoubleComplex(-1.0f, 0.0f);
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            rustHandle?.Dispose();
        }
    }

    [Benchmark]
    public void Managed()
    {
        Fft.Calculate(Log2FftSize, xyManaged, xyOutManaged);
    }

    [Benchmark(Baseline = true)]
    public void NativeC()
    {
        FftNativeC.Fft(Log2FftSize, xyNative, xyOutNative);
    }

    [Benchmark]
    public void NativeRust()
    {
        rustHandle.Fft(xyOutRust, xyRust, size);
    }
}