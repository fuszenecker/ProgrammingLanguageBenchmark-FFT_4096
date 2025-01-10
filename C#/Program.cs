using System;
using CommandLine;

namespace CSharpFftDemo;

internal static class Benchmark
{
    public static int Main(string[] args)
    {
        int result = Parser.Default
            .ParseArguments<Arguments>(args)
            .MapResult(ProcessArguments, errs => -1);

        Console.WriteLine($"GC Heap Size (MB): {GC.GetGCMemoryInfo().HeapSizeBytes / 1048576: #,##0.00}");
        Console.WriteLine($"GC 0 Collection Count: {GC.CollectionCount(0)}");
        Console.WriteLine($"GC 1 Collection Count: {GC.CollectionCount(1)}");
        Console.WriteLine($"GC 2 Collection Count: {GC.CollectionCount(2)}");
        Console.WriteLine($"GC Total Pause Duration: {GC.GetTotalPauseDuration()}");
        Console.WriteLine($"GC Pause Time Percentage: {GC.GetGCMemoryInfo().PauseTimePercentage:p}");

        return result;
    }

    private static int ProcessArguments(Arguments opts)
    {
        try
        {
            Console.WriteLine($"Log2FftSize: {opts.Log2FftSize}, Repeat: {opts.FftRepeat}");

            Defaults.Log2FftSize = opts.Log2FftSize;
            Defaults.FftRepeat = opts.FftRepeat;

            if (opts.DotnetBenchmark)
            {
                opts.ManagedBenchmark = false;
                opts.NativeBenchmark = false;
            }

            return ManualBenchmarks.RunBenchmarks(opts.DotnetBenchmark, opts.ManagedBenchmark, opts.NativeBenchmark);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unhandled exception: {e.Message}");
            return -4;
        }
    }
}
