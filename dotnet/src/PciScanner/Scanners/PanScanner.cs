using System.Text.RegularExpressions;
using PciScanner.Validators;

namespace PciScanner.Scanners;

public static class PanScanner
{
    // Matches plain digit sequences of 13-19 digits bounded by non-word characters.
    private static readonly Regex PanPattern = new(
        @"\b\d{13,19}\b",
        RegexOptions.NonBacktracking | RegexOptions.Compiled
    );

    public static bool ContainsPan(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        foreach (var match in PanPattern.EnumerateMatches(input))
        {
            if (LuhnValidator.IsValid(input.AsSpan(match.Index, match.Length)))
                return true;
        }

        return false;
    }
}
