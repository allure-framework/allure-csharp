using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureExpectedResultsHtmlAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureExpectedResultsHtmlAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task SetsTestExpectedResultsHtml()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureExpectedResultsHtmlAttribute("<p>Expected Results</p>");

        attr.Apply(tr);

        await Assert.That(attr.HtmlText).IsEqualTo("<p>Expected Results</p>");
        await Assert.That(tr.ExpectedResultsHtml).IsEqualTo("<p>Expected Results</p>");
    }

    [Test]
    public async Task AppendsTestExpectedResultsHtmlWithoutSeparator()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            ExpectedResultsHtml = "<p>First</p>",
        };

        new AllureExpectedResultsHtmlAttribute("<p>Second</p>") { Append = true }.Apply(tr);

        await Assert.That(tr.ExpectedResultsHtml)
            .IsEqualTo("<p>First</p><p>Second</p>");
    }

    [Test]
    public async Task AppendHandlesNullCurrentValue()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new AllureExpectedResultsHtmlAttribute("<p>Expected Results</p>") { Append = true }
            .Apply(tr);

        await Assert.That(tr.ExpectedResultsHtml).IsEqualTo("<p>Expected Results</p>");
    }

    [Test]
    public async Task DoesNothingIfValueIsNull()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            ExpectedResultsHtml = "<p>Original</p>",
        };

        new AllureExpectedResultsHtmlAttribute(null!).Apply(tr);

        await Assert.That(tr.ExpectedResultsHtml).IsEqualTo("<p>Original</p>");
    }

    [AllureExpectedResultsHtml("<p>base</p>")]
    private class Base;
    [AllureExpectedResultsHtml("<p>derived</p>")]
    private class Derived : Base;
}
