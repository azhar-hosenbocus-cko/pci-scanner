using System.Diagnostics;
using System.Text;
using PciScanner.Scanners;
using Xunit;

namespace PciScanner.Tests.Scanners;

public class PanScannerPerformanceTests
{
    // Adversarial input: a pattern that would cause catastrophic backtracking
    // in a naive NFA engine — repeated digit sequences with no valid PAN.
    private static string BuildAdversarialInput(int length)
    {
        var sb = new StringBuilder(length);
        while (sb.Length < length)
            sb.Append("123456789 ");
        return sb.ToString(0, length);
    }

    private static string BuildLargeInput(int length)
    {
        var sb = new StringBuilder(length);
        while (sb.Length < length)
            sb.Append("lorem ipsum dolor sit amet ");
        return sb.ToString(0, length);
    }

    // --- Option 3: never hangs on adversarial input ---

    [Fact]
    public async Task ContainsPan_CompletesWithinTimeout_OnAdversarialInput()
    {
        var input = BuildAdversarialInput(1_000_000); // 1MB of adversarial digits
        var timeout = TimeSpan.FromSeconds(2);

        using var cts = new CancellationTokenSource(timeout);
        var task = Task.Run(() => PanScanner.ContainsPan(input), cts.Token);

        var completed = await Task.WhenAny(task, Task.Delay(timeout)) == task;

        Assert.True(completed, "ContainsPan did not complete within the timeout — possible backtracking.");
        Assert.False(await task);
    }

    // --- Option 2: linear scaling ---

    [Fact]
    public void ContainsPan_ScalesLinearly_WithInputSize()
    {
        const int smallSize  = 10_000;    // 10KB
        const int largeSize  = 1_000_000; // 1MB — 100x larger

        var smallInput = BuildLargeInput(smallSize);
        var largeInput = BuildLargeInput(largeSize);

        // Warmup — ensure JIT doesn't skew the first measurement
        PanScanner.ContainsPan(smallInput);
        PanScanner.ContainsPan(largeInput);

        var sw = Stopwatch.StartNew();
        PanScanner.ContainsPan(smallInput);
        var smallElapsed = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        PanScanner.ContainsPan(largeInput);
        var largeElapsed = sw.Elapsed.TotalMilliseconds;

        // Input is 100x larger — allow up to 200x time to account for system noise.
        // Exponential backtracking would produce ratios in the thousands or more.
        var ratio = largeElapsed / smallElapsed;
        Assert.True(ratio < 200, $"Scaling ratio was {ratio:F1}x — expected near-linear (under 200x for 100x input). Possible backtracking.");
    }
}
