using System.Reflection;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureLinkAttributeTests
{
    [Test]
    public async Task CanBeQueriedFromBaseAndInheritedClasses()
    {
        var attrs = typeof(Derived).GetCustomAttributes<AllureLinkAttribute>();
        await Assert.That(attrs.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task UrlOnlyLinkCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureLinkAttribute("https://example.org/resource");

        attr.Apply(tr);

        await Assert.That(attr.Url).IsEqualTo("https://example.org/resource");
        await AttributeAssertions.AssertLink(
            tr,
            "https://example.org/resource",
            null,
            null
        );
    }

    [Test]
    public async Task LinkWithUrlTitleAndTypeCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new AllureLinkAttribute("https://example.org/resource")
        {
            Title = "Resource title",
            Type = "reference",
        }.Apply(tr);

        await AttributeAssertions.AssertLink(
            tr,
            "https://example.org/resource",
            "Resource title",
            "reference"
        );
    }

    [Test]
    public async Task LinkWithUrlAndTitleCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new AllureLinkAttribute("https://example.org/resource")
        {
            Title = "Resource title",
        }.Apply(tr);

        await AttributeAssertions.AssertLink(
            tr,
            "https://example.org/resource",
            "Resource title",
            null
        );
    }

    [Test]
    public async Task DoesNothingIfUrlIsNull()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new AllureLinkAttribute(null!).Apply(tr);

        await Assert.That(tr.Links).IsEmpty();
    }

    [AllureLink("https://example.org/base")]
    private class Base;
    [AllureLink("https://example.org/derived")]
    private class Derived : Base;
}
