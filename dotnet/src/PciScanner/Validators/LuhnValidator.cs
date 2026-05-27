namespace PciScanner.Validators;

internal static class LuhnValidator
{
    internal static bool IsValid(ReadOnlySpan<char> digits)
    {
        if (digits.Length is < 13 or > 19)
            return false;

        var sum = 0;
        var doubleIt = false;

        for (var i = digits.Length - 1; i >= 0; i--)
        {
            if (!char.IsAsciiDigit(digits[i]))
                return false;

            var digit = digits[i] - '0';

            if (doubleIt)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            doubleIt = !doubleIt;
        }

        return sum % 10 == 0;
    }
}
