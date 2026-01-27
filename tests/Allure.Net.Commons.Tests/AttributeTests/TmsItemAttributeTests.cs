using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class TmsItemAttributeTests
{
    [Test]
    public void UrlOnlyTmsItemCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureTmsItemAttribute("foo").Apply(tr);

        Assert.That(
            tr.links,
            Is.EquivalentTo([new Link { url = "foo", type = "tms" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void TmsItemWithTitleCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureTmsItemAttribute("foo")
        {
            Title = "bar",
        }.Apply(tr);

        Assert.That(
            tr.links,
            Is.EquivalentTo([new Link { url = "foo", name = "bar", type = "tms" }])
                .UsingPropertiesComparer()
        );
    }
}
