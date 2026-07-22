using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureDescriptionAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureDescriptionAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task SetsTestDescription()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureDescriptionAttribute("Description");

        attr.Apply(tr);

        await Assert.That(attr.MarkdownText).IsEqualTo("Description");
        await Assert.That(tr.Description).IsEqualTo("Description");
    }

    [Test]
    public async Task AppendsTestDescription()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            Description = "First",
        };

        new AllureDescriptionAttribute("Second") { Append = true }.Apply(tr);

        await Assert.That(tr.Description).IsEqualTo("First\n\nSecond");
    }

    [Test]
    public async Task OmitsSeparatorWhenAppendingToNullDescription()
    {
        TestResult nullDescription = new() { Name = "test", Uuid = "id" };
        var attr = new AllureDescriptionAttribute("Description") { Append = true };

        attr.Apply(nullDescription);

        await Assert.That(nullDescription.Description).IsEqualTo("Description");
    }

    [Test]
    public async Task OmitsSeparatorWhenAppendingToEmptyDescription()
    {
        TestResult emptyDescription = new()
        {
            Name = "test",
            Uuid = "id",
            Description = "",
        };

        new AllureDescriptionAttribute("Description") { Append = true }
            .Apply(emptyDescription);

        await Assert.That(emptyDescription.Description).IsEqualTo("Description");
    }

    [Test]
    public async Task DoesNothingIfDescriptionIsNull()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            Description = "Original",
        };

        new AllureDescriptionAttribute(null!).Apply(tr);

        await Assert.That(tr.Description).IsEqualTo("Original");
    }

    [AllureDescription("base")]
    private class Base;
    [AllureDescription("derived")]
    private class Derived : Base;
}
