using PciScanner.Scanners;
using Xunit;

namespace PciScanner.Tests.Scanners;

public class PanScannerTests
{
    // --- static method ---

    [Theory]
    [InlineData("4111111111111111")]                           // bare PAN
    [InlineData("order=abc&card=4111111111111111&exp=12/26")]  // embedded in query string
    [InlineData("some log text 378282246310005 more text")]    // embedded in text
    public void ContainsPan_ReturnsTrue_ForPlainPan(string input)
    {
        Assert.True(PanScanner.ContainsPan(input));
    }

    [Theory]
    [InlineData("")]
    [InlineData("hello world")]
    [InlineData("1234567890123456")]        // 16 digits but fails Luhn
    [InlineData("order id is 1234567890")]  // too short
    public void ContainsPan_ReturnsFalse_WhenNoPan(string input)
    {
        Assert.False(PanScanner.ContainsPan(input));
    }

    [Fact]
    public void ContainsPan_ReturnsFalse_ForNull()
    {
        Assert.False(PanScanner.ContainsPan(null!));
    }

    // --- extension method ---

    [Fact]
    public void ContainsPan_Extension_ReturnsTrue_ForValidPan()
    {
        Assert.True("4111111111111111".ContainsPan());
    }

    [Fact]
    public void ContainsPan_Extension_ReturnsFalse_WhenNoPan()
    {
        Assert.False("hello world".ContainsPan());
    }
}
