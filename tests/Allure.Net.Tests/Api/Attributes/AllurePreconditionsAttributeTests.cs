using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllurePreconditionsAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllurePreconditionsAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task SetsTestPreconditions()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllurePreconditionsAttribute("Preconditions");

        attr.Apply(tr);

        await Assert.That(attr.MarkdownText).IsEqualTo("Preconditions");
        await Assert.That(tr.Preconditions).IsEqualTo("Preconditions");
    }

    [Test]
    public async Task AppendsTestPreconditions()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            Preconditions = "First",
        };

        new AllurePreconditionsAttribute("Second") { Append = true }.Apply(tr);

        await Assert.That(tr.Preconditions).IsEqualTo("First\n\nSecond");
    }

    [Test]
    public async Task OmitsSeparatorWhenAppendingToNullPreconditions()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllurePreconditionsAttribute("Preconditions") { Append = true };

        attr.Apply(tr);

        await Assert.That(tr.Preconditions).IsEqualTo("Preconditions");
    }

    [Test]
    public async Task OmitsSeparatorWhenAppendingToEmptyPreconditions()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            Preconditions = "",
        };

        new AllurePreconditionsAttribute("Preconditions") { Append = true }
            .Apply(tr);

        await Assert.That(tr.Preconditions).IsEqualTo("Preconditions");
    }

    [Test]
    public async Task DoesNothingIfPreconditionsAreNull()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            Preconditions = "Original",
        };

        new AllurePreconditionsAttribute(null!).Apply(tr);

        await Assert.That(tr.Preconditions).IsEqualTo("Original");
    }

    [AllurePreconditions("base")]
    private class Base;
    [AllurePreconditions("derived")]
    private class Derived : Base;
}
