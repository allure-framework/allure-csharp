using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class TmsItemAttributeTests
{
    [AllureTmsItem("foo")]
    class TargetBase { }

    [AllureTmsItem("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CanBeQueriedFromBaseAndInheritedClasses()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureTmsItemAttribute>();

        Assert.That(typeAttributes, Has.Exactly(2).Items);
    }

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

    [Test]
    public void DoesNothingIfUrlIsNull()
    {
        TestResult tr = new();

        new AllureTmsItemAttribute(null).Apply(tr);

        Assert.That(tr.links, Is.Empty);
    }
}
