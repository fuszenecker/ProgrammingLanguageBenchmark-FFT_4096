using System;
using CommandLine;

namespace CSharpFftDemo;

internal static class Benchmark
{
    public static int Main(string[] args)
    {
        return Parser.Default
            .ParseArguments<Arguments>(args)
            .MapResult(ProcessArguments, errs => -1);
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
