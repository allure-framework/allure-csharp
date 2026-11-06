using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureExpectedResultsAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureExpectedResultsAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task SetsTestExpectedResults()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureExpectedResultsAttribute("ExpectedResults");

        attr.Apply(tr);

        await Assert.That(attr.MarkdownText).IsEqualTo("ExpectedResults");
        await Assert.That(tr.ExpectedResults).IsEqualTo("ExpectedResults");
    }

    [Test]
    public async Task AppendsTestExpectedResults()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            ExpectedResults = "First",
        };

        new AllureExpectedResultsAttribute("Second") { Append = true }.Apply(tr);

        await Assert.That(tr.ExpectedResults).IsEqualTo("First\n\nSecond");
    }

    [Test]
    public async Task OmitsSeparatorWhenAppendingToNullExpectedResults()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureExpectedResultsAttribute("ExpectedResults") { Append = true };

        attr.Apply(tr);

        await Assert.That(tr.ExpectedResults).IsEqualTo("ExpectedResults");
    }

    [Test]
    public async Task OmitsSeparatorWhenAppendingToEmptyExpectedResults()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            ExpectedResults = "",
        };

        new AllureExpectedResultsAttribute("ExpectedResults") { Append = true }
            .Apply(tr);

        await Assert.That(tr.ExpectedResults).IsEqualTo("ExpectedResults");
    }

    [Test]
    public async Task DoesNothingIfExpectedResultsAreNull()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            ExpectedResults = "Original",
        };

        new AllureExpectedResultsAttribute(null!).Apply(tr);

        await Assert.That(tr.ExpectedResults).IsEqualTo("Original");
    }

    [AllureExpectedResults("base")]
    private class Base;
    [AllureExpectedResults("derived")]
    private class Derived : Base;
}
