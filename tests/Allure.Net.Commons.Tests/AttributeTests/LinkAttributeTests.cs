using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class LinkAttributeTests
{
    [AllureLink("foo")]
    class TargetBase { }

    [AllureLink("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CanBeQueriedFromBaseAndInheritedClasses()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureLinkAttribute>();

        Assert.That(typeAttributes, Has.Exactly(2).Items);
    }

    [Test]
    public void UrlOnlyLinkCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureLinkAttribute("foo");

        attr.Apply(tr);

        Assert.That(attr.Url, Is.EqualTo("foo"));
        Assert.That(
            tr.links,
            Is.EquivalentTo([new Link { url = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void LinkWithUrlAndTitleCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureLinkAttribute("foo")
        {
            Title = "bar",
        }.Apply(tr);

        Assert.That(
            tr.links,
            Is.EquivalentTo([new Link { url = "foo", name = "bar" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void LinkWithUrlTitleAndTypeCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureLinkAttribute("foo")
        {
            Title = "bar",
            Type = "baz",
        }.Apply(tr);

        Assert.That(
            tr.links,
            Is.EquivalentTo([new Link { url = "foo", name = "bar", type = "baz" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void DoesNothingIfUrlIsNull()
    {
        TestResult tr = new();

        new AllureLinkAttribute(null).Apply(tr);

        Assert.That(tr.links, Is.Empty);
    }
}
