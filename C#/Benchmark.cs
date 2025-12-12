using System;

using static CSharpFftDemo.GlobalResourceManager;

namespace CSharpFftDemo;

internal sealed class Arguments
{
    public bool DotnetBenchmark { get; set; } = false;
    public bool ManagedBenchmark { get; set; } = true;
    public bool NativeBenchmark { get; set; } = false;
    public int FftRepeat { get; set; } = 20000;
    public int Log2FftSize { get; set; } = 12;
}

internal static class ArgumentParser
{
    public static Arguments Parse(string[] args)
    {
        var arguments = new Arguments();

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];

            if (arg == "-d" || arg == "--dotnet-benchmark")
            {
                arguments.DotnetBenchmark = true;
            }
            else if (arg == "-m" || arg == "--managed")
            {
                arguments.ManagedBenchmark = true;
            }
            else if (arg == "-n" || arg == "--native")
            {
                arguments.NativeBenchmark = true;
            }
            else if (arg == "-r" || arg == "--repeat")
            {
                if (i + 1 < args.Length && int.TryParse(args[i + 1], out int repeat))
                {
                    arguments.FftRepeat = repeat;
                    i++;
                }
            }
            else if (arg == "-s" || arg == "--size")
            {
                if (i + 1 < args.Length && int.TryParse(args[i + 1], out int size))
                {
                    arguments.Log2FftSize = size;
                    i++;
                }
            }
            else if (arg == "-h" || arg == "--help")
            {
                PrintHelp();
                Environment.Exit(0);
            }
        }

        return arguments;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("FFT Benchmark Tool");
        Console.WriteLine("Usage: fft-benchmark [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -d, --dotnet-benchmark    Run the BenchmarkDotNet benchmark");
        Console.WriteLine("  -m, --managed              Run the .NET managed benchmark (default)");
        Console.WriteLine("  -n, --native               Run the native (C-fast_double) benchmark");
        Console.WriteLine("  -r, --repeat <N>           Number of iterations (default: 20000)");
        Console.WriteLine("  -s, --size <N>             Log2 of buffer size (default: 12 for 4096 samples)");
        Console.WriteLine("  -h, --help                 Show this help message");
    }
}

internal static class Benchmark
{
    public static int Main(string[] args)
    {
        try
        {
            var opts = ArgumentParser.Parse(args);
            Console.WriteLine($"Log2FftSize: {opts.Log2FftSize}, Repeat: {opts.FftRepeat}");

            Params.Log2FftSize = opts.Log2FftSize;
            Params.FftRepeat = opts.FftRepeat;

            return Benchmarks(opts.DotnetBenchmark, opts.ManagedBenchmark, opts.NativeBenchmark);
        }
        catch (Exception e)
        {
            Console.WriteLine(GetStringResource("UnhandledExceptionText")!, e.Message);
            return -4;
        }
    }

    private static int Benchmarks(bool dotnetBenchmark, bool managedBenchmark, bool nativeBenchmark)
    {
        double? managedElapsedMillisecond = null;
        double? nativeElapsedMillisecond = null;

        if (dotnetBenchmark)
        {
            // Benchmark
            DotnetBenchmark.Calculate();
        }

        if (managedBenchmark)
        {
            // Warmup
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(GetStringResource("ManagedWarmupText"));
            Console.ForegroundColor = ConsoleColor.Gray;

            FftManaged.WarmUp(Params.Log2FftSize, Params.FftRepeat);

            // Benchmark
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(GetStringResource("ManagedText"));
            Console.ForegroundColor = ConsoleColor.Gray;

            managedElapsedMillisecond = FftManaged.Calculate(Params.Log2FftSize, Params.FftRepeat);
        }

        if (nativeBenchmark)
        {
            try
            {
                // Benchmark
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(GetStringResource("NativeText"));
                Console.ForegroundColor = ConsoleColor.Gray;

                nativeElapsedMillisecond = FftNative.Calculate(Params.Log2FftSize, Params.FftRepeat);
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine(GetStringResource("CanotRunNative")!, e.Message);
                Console.WriteLine(GetStringResource("HaveYouCompiledNative"));
                return 1;
            }
        }

        if (managedElapsedMillisecond.HasValue && nativeElapsedMillisecond.HasValue)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Native Ratio: {managedElapsedMillisecond / nativeElapsedMillisecond:0.####}");
            Console.WriteLine($"Native Diff%: {managedElapsedMillisecond / nativeElapsedMillisecond - 1:0.##%}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        return 0;
    }
}
