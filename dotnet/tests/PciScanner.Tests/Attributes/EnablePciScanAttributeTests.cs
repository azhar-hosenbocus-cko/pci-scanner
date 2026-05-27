using System.ComponentModel.DataAnnotations;
using PciScanner.Attributes;
using Xunit;

namespace PciScanner.Tests.Attributes;

public class EnablePciScanAttributeTests
{
    [EnablePciScan]
    private class Model
    {
        [SkipPciScan]
        public string? CardNumber { get; set; }

        public string? Description { get; set; }
        public string? Notes { get; set; }
    }

    private static IList<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Valid_WhenNoScannedPropertiesContainPan()
    {
        var model = new Model
        {
            CardNumber = "4111111111111111",  // skipped
            Description = "a normal description",
            Notes = "some notes"
        };

        Assert.Empty(Validate(model));
    }

    [Fact]
    public void Invalid_WhenScannedPropertyContainsPan()
    {
        var model = new Model
        {
            Description = "card is 4111111111111111"
        };

        var results = Validate(model);
        Assert.Single(results);
        Assert.Contains("Description", results[0].MemberNames);
    }

    [Fact]
    public void Invalid_ReportsAllFailingProperties()
    {
        var model = new Model
        {
            Description = "4111111111111111",
            Notes = "4111111111111111"
        };

        var results = Validate(model);
        Assert.Single(results);
        Assert.Contains("Description", results[0].MemberNames);
        Assert.Contains("Notes", results[0].MemberNames);
    }

    [Fact]
    public void Valid_WhenAllScannedPropertiesAreNull()
    {
        var model = new Model();
        Assert.Empty(Validate(model));
    }
}
