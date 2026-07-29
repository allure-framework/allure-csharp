using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureDescriptionHtmlAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureDescriptionHtmlAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task SetsTestDescription()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureDescriptionHtmlAttribute("<p>Description</p>");

        attr.Apply(tr);

        await Assert.That(attr.HtmlText).IsEqualTo("<p>Description</p>");
        await Assert.That(tr.DescriptionHtml).IsEqualTo("<p>Description</p>");
    }

    [Test]
    public async Task AppendsTestDescriptionWithoutSeparator()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            DescriptionHtml = "<p>First</p>",
        };

        new AllureDescriptionHtmlAttribute("<p>Second</p>") { Append = true }.Apply(tr);

        await Assert.That(tr.DescriptionHtml)
            .IsEqualTo("<p>First</p><p>Second</p>");
    }

    [Test]
    public async Task AppendHandlesNullCurrentValue()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new AllureDescriptionHtmlAttribute("<p>Description</p>") { Append = true }
            .Apply(tr);

        await Assert.That(tr.DescriptionHtml).IsEqualTo("<p>Description</p>");
    }

    [Test]
    public async Task DoesNothingIfValueIsNull()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            DescriptionHtml = "<p>Original</p>",
        };

        new AllureDescriptionHtmlAttribute(null!).Apply(tr);

        await Assert.That(tr.DescriptionHtml).IsEqualTo("<p>Original</p>");
    }

    [AllureDescriptionHtml("<p>base</p>")]
    private class Base;
    [AllureDescriptionHtml("<p>derived</p>")]
    private class Derived : Base;
}
