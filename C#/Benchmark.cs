﻿using System;
using System.CommandLine;
using System.IO;
using MathNet.Numerics;

namespace CSharpFftDemo;

public static class Benchmark
{
    private static int Main(string[] args)
    {
        var dotnetBenchmarkOption = new Option<bool>(
            new [] {
                "-d",
                "--dotnet-benchmark"
            },
            getDefaultValue: () => false,
            "Run the BenchmarkDotNet benchmark");

        var managedBenchmarkOption = new Option<bool>(
            new [] {
                "-m",
                "--managed"
            },
            getDefaultValue: () => true,
            "Run the .NET managed benchmark");

        var nativeBenchmarkOption = new Option<bool>(
            new [] {
                "-n",
                "--native"
            },
            getDefaultValue: () => File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}libfft.so"),
            "Run the native (C-fast_double) benchmark");

        var mathNetBenchmarkOption = new Option<bool>(
            new [] {
                "-M",
                "--mathnet"
            },
            getDefaultValue: () => true,
            "Run the MathNet benchmark");

        var repeatOption = new Option<int>(
            new [] {
                "-r",
                "--repeat"
            },
            getDefaultValue: () => 20000,
            "Number of iterations, e.g. 20000.");

        var log2FftSizeOption = new Option<int>(
            new [] {
                "-s",
                "--size"
            },
            getDefaultValue: () => 12,
            "Log2 of the buffer size, e.g. 12 for 4096 samples.");

        var rootCommand = new RootCommand
        {
            dotnetBenchmarkOption,
            managedBenchmarkOption,
            nativeBenchmarkOption,
            mathNetBenchmarkOption,
            log2FftSizeOption,
            repeatOption
        };

        rootCommand.Description = "FFT Benchmark from Zsolt Krüpl.";

        rootCommand.SetHandler(
            (bool dotnetBenchmark, bool  managedBenchmark, bool nativeBenchmark, bool mathNetBenchmark,
                int log2FftSize, int repeat) => {
                    Console.WriteLine("Log2FftSize: {0}, Repeat: {1}", log2FftSize, repeat);

                    Params.Log2FftSize = log2FftSize;
                    Params.FftRepeat = repeat;

                    SetupMathNet();
                    Benchmarks(dotnetBenchmark, managedBenchmark, nativeBenchmark, mathNetBenchmark);
                },
                dotnetBenchmarkOption, managedBenchmarkOption, nativeBenchmarkOption, mathNetBenchmarkOption,
                log2FftSizeOption, repeatOption);

        return rootCommand.Invoke(args);
    }

    private static void SetupMathNet()
    {
        Console.WriteLine("Setting up Math.NET: ");

        if (Control.TryUseNativeCUDA())
        {
            Console.WriteLine("  - Using CUDA engine.");
            return;
        }
        else if (Control.TryUseNativeMKL())
        {
            Console.WriteLine("  - Using MKL engine.");
            return;
        }
        else if (Control.TryUseNativeOpenBLAS())
        {
            Console.WriteLine("  - Using OpenBLAS engine.");
            return;
        }
        else if (Control.TryUseNative())
        {
            Console.WriteLine("  - Using native provider.");
            return;
        }
        else
        {
            Console.WriteLine("  - Using managed provider.");
            Console.WriteLine("  - Enabling multithreading");
            Control.UseMultiThreading();
        }
    }

    private static int Benchmarks(bool dotnetBenchmark, bool managedBenchmark, bool nativeBenchmark, bool mathNetBenchmark)
    {
        double? managedElapsedMillisecond = null;
        double? nativeElapsedMillisecond = null;
        double? mathNetElapsedMillisecond = null;

        if (dotnetBenchmark)
        {
            // Benchmark
            DotnetBenchmark.Calculate();
        }

        if (managedBenchmark)
        {
            // Warmup
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("---- MANAGED (warmup) ----");
            Console.ForegroundColor = ConsoleColor.Gray;

            FftManaged.WarmUp(Params.Log2FftSize, Params.FftRepeat);

            // Benchmark
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("---- MANAGED ----");
            Console.ForegroundColor = ConsoleColor.Gray;

            managedElapsedMillisecond = FftManaged.Calculate(Params.Log2FftSize, Params.FftRepeat);
        }

        if (nativeBenchmark)
        {
            try
            {
                // Benchmark
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("---- NATIVE ----");
                Console.ForegroundColor = ConsoleColor.Gray;

                nativeElapsedMillisecond = FftNative.Calculate(Params.Log2FftSize, Params.FftRepeat);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Can not run native method: {e.Message}");
                Console.WriteLine("Have you successfully compiled the project in ../C-tests/C-fast_double/?");
                return 1;
            }
        }

        if (mathNetBenchmark)
        {
            // Warmup
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("---- MATH.NET (warmup) ----");
            Console.ForegroundColor = ConsoleColor.Gray;

            FftMathNet.WarmUp(Params.Log2FftSize, Params.FftRepeat);

            // Benchmark
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("---- MATH.NET ----");
            Console.ForegroundColor = ConsoleColor.Gray;

            mathNetElapsedMillisecond = FftMathNet.Calculate(Params.Log2FftSize, Params.FftRepeat);
        }

        if (managedElapsedMillisecond.HasValue && nativeElapsedMillisecond.HasValue)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Native Ratio: {managedElapsedMillisecond / nativeElapsedMillisecond:0.####}");
            Console.WriteLine($"Native Diff%: {managedElapsedMillisecond / nativeElapsedMillisecond - 1:0.##%}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        if (managedElapsedMillisecond.HasValue && mathNetElapsedMillisecond.HasValue)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nMathNet Ratio: {mathNetElapsedMillisecond / nativeElapsedMillisecond:0.####}");
            Console.WriteLine($"MathNet Diff%: {mathNetElapsedMillisecond / nativeElapsedMillisecond - 1:0.##%}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        return 0;
    }
}
