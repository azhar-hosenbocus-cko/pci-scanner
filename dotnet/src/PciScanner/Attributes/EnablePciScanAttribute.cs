using System.ComponentModel.DataAnnotations;
using System.Reflection;
using PciScanner.Scanners;
using PciScanner.Validators;

namespace PciScanner.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class EnablePciScanAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        var failingProperties = new List<string>();

        foreach (var property in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.PropertyType != typeof(string))
                continue;

            if (property.GetCustomAttribute<SkipPciScanAttribute>() is not null)
                continue;

            if (property.GetValue(value) is not string propertyValue)
                continue;

            if (PanScanner.ContainsPan(propertyValue))
                failingProperties.Add(property.Name);
        }

        if (failingProperties.Count == 0)
            return ValidationResult.Success;

        return new ValidationResult(
            $"PAN detected in: {string.Join(", ", failingProperties)}",
            failingProperties
        );
    }
}
