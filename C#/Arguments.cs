using CommandLine;

internal sealed class Arguments
{
    [Option(shortName: 'd', longName: "dotnet-benchmark", Default = false,
        Required = false, HelpText = "Run the BenchmarkDotNet benchmark (disabled all other ones)")]
    public bool DotnetBenchmark { get; set; }

    [Option(shortName: 'm', longName: "managed", Default = true,
        Required = false, HelpText = "Run the .NET managed benchmark")]
    public bool ManagedBenchmark { get; set; }

    [Option(shortName: 'n', longName: "native", Default = false,
        Required = false, HelpText = "Run the native (C and Rust) benchmarks")]
    public bool NativeBenchmark { get; set; }

    [Option(shortName: 'r', longName: "repeat", Default = 20000,
        Required = false, HelpText = "Number of iterations, e.g. 20000")]
    public int FftRepeat { get; set; }

    [Option(shortName: 's', longName: "size", Default = 12,
        Required = false, HelpText = "Log2 of the buffer size, e.g. 12 for 4096 samples")]
    public int Log2FftSize { get; set; }

    public Arguments()
    {
    }
}