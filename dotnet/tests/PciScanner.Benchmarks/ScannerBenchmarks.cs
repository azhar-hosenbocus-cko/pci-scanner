using System.ComponentModel.DataAnnotations;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using PciScanner.Attributes;
using PciScanner.Scanners;

namespace PciScanner.Benchmarks;

[Config(typeof(Config))]
[MemoryDiagnoser]
public class ScannerBenchmarks
{
    private class Config : ManualConfig
    {
        public Config()
        {
            AddJob(Job.Default.WithId("net8").WithEnvironmentVariable("DOTNET_ROLL_FORWARD", "LatestMajor"));
        }
    }

    [EnablePciScan]
    private class ModelWithNoPan
    {
        [SkipPciScan] public string? CardNumber { get; set; } = "4111111111111111";
        public string? Description { get; set; } = "a normal description";
        public string? Notes { get; set; } = "some notes";
    }

    [EnablePciScan]
    private class ModelWithPan
    {
        [SkipPciScan] public string? CardNumber { get; set; } = "4111111111111111";
        public string? Description { get; set; } = "4111111111111111";
        public string? Notes { get; set; } = "some notes";
    }

    private static readonly ModelWithNoPan _modelWithNoPan = new();
    private static readonly ModelWithPan _modelWithPan = new();
    private static readonly List<ValidationResult> _results = [];

    private const string NoPan         = "hello world, no sensitive data here";
    private const string PlainPan      = "4111111111111111";
    private const string PanInText     = "user=john&card=4111111111111111&action=purchase&amount=99.99";
    private const string LongTextNoPan = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. " +
                                         "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. " +
                                         "Reference number: 9999999999999999. No card here.";

    // --- PanScanner ---

    [Benchmark(Baseline = true)]
    public bool NoMatch_ShortString() => PanScanner.ContainsPan(NoPan);

    [Benchmark]
    public bool NoMatch_LongString() => PanScanner.ContainsPan(LongTextNoPan);

    [Benchmark]
    public bool PlainPan_Match() => PanScanner.ContainsPan(PlainPan);

    [Benchmark]
    public bool PanEmbedded_Match() => PanScanner.ContainsPan(PanInText);

    [Benchmark]
    public bool Extension_Match() => PlainPan.ContainsPan();

    // --- EnablePciScan attribute ---

    [Benchmark]
    public bool Attribute_NoMatch()
    {
        _results.Clear();
        return Validator.TryValidateObject(_modelWithNoPan, new ValidationContext(_modelWithNoPan), _results, validateAllProperties: true);
    }

    [Benchmark]
    public bool Attribute_Match()
    {
        _results.Clear();
        return Validator.TryValidateObject(_modelWithPan, new ValidationContext(_modelWithPan), _results, validateAllProperties: true);
    }
}
