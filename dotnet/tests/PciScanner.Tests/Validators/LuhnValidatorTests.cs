using Xunit;
using PciScanner.Validators;

namespace PciScanner.Tests.Validators;

public class LuhnValidatorTests
{
    [Theory]
    [InlineData("4111111111111111")]  // Visa test number
    [InlineData("5500005555555559")]  // Mastercard test number
    [InlineData("378282246310005")]   // Amex test number
    [InlineData("6011111111111117")]  // Discover test number
    public void IsValid_ReturnsTrue_ForKnownValidNumbers(string number)
    {
        Assert.True(LuhnValidator.IsValid(number.AsSpan()));
    }

    [Theory]
    [InlineData("4111111111111112")]  // valid Visa with last digit changed
    [InlineData("1234567890123456")]  // random digits
    [InlineData("9999999999999999")]
    public void IsValid_ReturnsFalse_ForInvalidNumbers(string number)
    {
        Assert.False(LuhnValidator.IsValid(number.AsSpan()));
    }

    [Theory]
    [InlineData("123456789012")]      // 12 digits — too short
    [InlineData("12345678901234567890")] // 20 digits — too long
    [InlineData("")]
    public void IsValid_ReturnsFalse_ForOutOfRangeLengths(string number)
    {
        Assert.False(LuhnValidator.IsValid(number.AsSpan()));
    }
}
