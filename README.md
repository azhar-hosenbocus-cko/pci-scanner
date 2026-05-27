# pci-scanner

A library for detecting PCI-sensitive data (PANs) in strings and object models.

Uses a DFA-based regex engine (`RegexOptions.NonBacktracking`) with Luhn validation — O(n) time guaranteed, no catastrophic backtracking possible.

## Supported frameworks

- .NET 8
- .NET 9
- .NET 10

## Namespaces

| Namespace | Use when |
|---|---|
| `PciScanner.Scanners` | Scanning raw strings |
| `PciScanner.Attributes` | Validating object models |

## Installation

```bash
dotnet add package PciScanner
```

## Usage

### Raw string scanning

```csharp
using PciScanner.Scanners;

// Static
bool hasPan = PanScanner.ContainsPan("some text 4111111111111111 more text");

// Extension
bool hasPan = "some text 4111111111111111 more text".ContainsPan();
```

### Model validation

Decorate your class with `[EnablePciScan]`. All `string` properties are scanned by default. Use `[SkipPciScan]` on properties that are allowed to hold a PAN.

```csharp
using PciScanner.Attributes;

[EnablePciScan]
public class PaymentRequest
{
    [SkipPciScan]
    public string CardNumber { get; set; }   // holds a PAN intentionally — skipped

    public string Description { get; set; }  // must not contain a PAN — scanned
    public string Notes { get; set; }        // must not contain a PAN — scanned
}
```

Validation integrates with `System.ComponentModel.DataAnnotations` and works automatically with ASP.NET Core model binding:

```csharp
var request = new PaymentRequest
{
    CardNumber  = "4111111111111111",
    Description = "4111111111111111"  // PAN leaked into description
};

var results = new List<ValidationResult>();
bool isValid = Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
// isValid = false
// results[0].ErrorMessage = "PAN detected in: Description"
```
