using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class TagAttributeTests
{
    [AllureTag("foo")]
    class TargetBase { }

    [AllureTag("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CanBeQueriedFromBaseAndInheritedClasses()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureTagAttribute>();

        Assert.That(typeAttributes, Has.Exactly(2).Items);
    }

    [Test]
    public void ItAddsSingleTagToTest()
    {
        TestResult tr = new();

        new AllureTagAttribute("foo").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "tag", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void ItAddsMultipleTagsToTest()
    {
        TestResult tr = new();

        new AllureTagAttribute("foo", "bar", "baz").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "tag", value = "foo" },
                new Label { name = "tag", value = "bar" },
                new Label { name = "tag", value = "baz" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void NullTagsAreIgnored()
    {
        TestResult tr = new();

        new AllureTagAttribute(null, null, null).Apply(tr);

        Assert.That(tr.labels, Is.Empty);
    }

    [Test]
    public void EmptyTagsAreIgnored()
    {
        TestResult tr = new();

        new AllureTagAttribute("", "", "").Apply(tr);

        Assert.That(tr.labels, Is.Empty);
    }
}