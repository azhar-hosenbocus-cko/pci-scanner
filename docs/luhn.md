# Luhn Algorithm

The Luhn algorithm is a checksum formula used to validate card numbers. Every real PAN passes it; random digit sequences almost never do.

## Steps

Given a digit string, from right to left:
1. Keep odd-position digits (1st, 3rd, 5th...) as-is
2. Double even-position digits (2nd, 4th, 6th...); if the result > 9, subtract 9
3. Sum all digits
4. Valid if `sum % 10 == 0`

## Example — `4111111111111111`

```
Position (RTL): 1  2  1  2  1  2  1  2  1  2  1  2  1  2  1  2
Digit:          1  1  1  1  1  1  1  1  1  1  1  1  1  1  1  4
After step:     1  2  1  2  1  2  1  2  1  2  1  2  1  2  1  8
Sum = 1+2+1+2+1+2+1+2+1+2+1+2+1+2+1+8 = 30 → 30 % 10 == 0 ✓
```

## Notes

- Luhn does not authenticate a card — it only detects transcription errors and random strings.
- All implementations across languages must produce identical results for the same input.
- Valid length range for PANs: 13–19 digits (ISO/IEC 7812).
