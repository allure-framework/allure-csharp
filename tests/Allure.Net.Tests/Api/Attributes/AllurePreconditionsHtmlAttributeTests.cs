using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllurePreconditionsHtmlAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllurePreconditionsHtmlAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task SetsTestPreconditionsHtml()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllurePreconditionsHtmlAttribute("<p>Preconditions</p>");

        attr.Apply(tr);

        await Assert.That(attr.HtmlText).IsEqualTo("<p>Preconditions</p>");
        await Assert.That(tr.PreconditionsHtml).IsEqualTo("<p>Preconditions</p>");
    }

    [Test]
    public async Task AppendsTestPreconditionsHtmlWithoutSeparator()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            PreconditionsHtml = "<p>First</p>",
        };

        new AllurePreconditionsHtmlAttribute("<p>Second</p>") { Append = true }.Apply(tr);

        await Assert.That(tr.PreconditionsHtml)
            .IsEqualTo("<p>First</p><p>Second</p>");
    }

    [Test]
    public async Task AppendHandlesNullCurrentValue()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new AllurePreconditionsHtmlAttribute("<p>Preconditions</p>") { Append = true }
            .Apply(tr);

        await Assert.That(tr.PreconditionsHtml).IsEqualTo("<p>Preconditions</p>");
    }

    [Test]
    public async Task DoesNothingIfValueIsNull()
    {
        TestResult tr = new()
        {
            Name = "test",
            Uuid = "id",
            PreconditionsHtml = "<p>Original</p>",
        };

        new AllurePreconditionsHtmlAttribute(null!).Apply(tr);

        await Assert.That(tr.PreconditionsHtml).IsEqualTo("<p>Original</p>");
    }

    [AllurePreconditionsHtml("<p>base</p>")]
    private class Base;
    [AllurePreconditionsHtml("<p>derived</p>")]
    private class Derived : Base;
}
