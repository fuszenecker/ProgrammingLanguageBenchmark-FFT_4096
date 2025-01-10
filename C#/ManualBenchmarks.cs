using System;

namespace CSharpFftDemo;

internal static class ManualBenchmarks
{
    internal static int RunBenchmarks(bool dotnetBenchmark, bool managedBenchmark, bool nativeBenchmark)
    {
        double? managedElapsedMillisecond = null;
        double? nativeCElapsedMillisecond = null;
        double? nativeRustElapsedMillisecond = null;

        if (dotnetBenchmark)
        {
            // Benchmark
            DotnetBenchmarks.Calculate();
        }

        if (managedBenchmark)
        {
            // Warmup
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("---- MANAGED (warmup) ----");
            Console.ForegroundColor = ConsoleColor.Gray;

            FftManaged.WarmUp(Defaults.Log2FftSize, Defaults.FftRepeat);

            // Benchmark
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("---- MANAGED ----");
            Console.ForegroundColor = ConsoleColor.Gray;

            managedElapsedMillisecond = FftManaged.Calculate(Defaults.Log2FftSize, Defaults.FftRepeat);
        }

        if (nativeBenchmark)
        {
            try
            {
                // Benchmark
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("---- NATIVE C ----");
                Console.ForegroundColor = ConsoleColor.Gray;

                nativeCElapsedMillisecond = FftNativeC.Calculate(Defaults.Log2FftSize, Defaults.FftRepeat);
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine($"Can not run native method: {e.Message}");       
                Console.WriteLine("Have you successfully compiled the native libraries?");
                return 1;
            }

            try
            {
                // Benchmark
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("---- NATIVE RUST ----");
                Console.ForegroundColor = ConsoleColor.Gray;

                nativeRustElapsedMillisecond = FftNativeRust.Calculate(Defaults.Log2FftSize, Defaults.FftRepeat);
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine($"Can not run native method: {e.Message}");       
                Console.WriteLine("Have you successfully compiled the native libraries?");
                return 2;
            }
        }

        if (managedElapsedMillisecond.HasValue && nativeCElapsedMillisecond.HasValue && nativeRustElapsedMillisecond.HasValue)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Native C Ratio    : {managedElapsedMillisecond / nativeCElapsedMillisecond:0.####}");
            Console.WriteLine($"Native C Diff%    : {managedElapsedMillisecond / nativeCElapsedMillisecond - 1:0.##%}");
            Console.WriteLine($"Native Rust Ratio : {managedElapsedMillisecond / nativeRustElapsedMillisecond:0.####}");
            Console.WriteLine($"Native Rust Diff% : {managedElapsedMillisecond / nativeRustElapsedMillisecond - 1:0.##%}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        return 0;
    }
}