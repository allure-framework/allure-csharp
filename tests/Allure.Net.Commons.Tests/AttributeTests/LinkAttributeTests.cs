using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class LinkAttributeTests
{
    [Test]
    public void UrlOnlyLinkCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureLinkAttribute("foo").Apply(tr);

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
}
