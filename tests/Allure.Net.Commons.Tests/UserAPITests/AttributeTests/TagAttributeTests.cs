using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AttributeTests;

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
        var attr = new AllureTagAttribute("foo");

        attr.Apply(tr);

        Assert.That(attr.Tags, Is.EqualTo(["foo"]));
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
        var attr = new AllureTagAttribute("foo", "bar", "baz");

        attr.Apply(tr);

        Assert.That(attr.Tags, Is.EqualTo(["foo", "bar", "baz"]));
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
        var attr = new AllureTagAttribute(null, null, null);

        attr.Apply(tr);

        Assert.That(attr.Tags, Is.EqualTo(new string[]{ null, null, null }));
        Assert.That(tr.labels, Is.Empty);
    }

    [Test]
    public void EmptyTagsAreIgnored()
    {
        TestResult tr = new();
        var attr = new AllureTagAttribute("", "", "");

        attr.Apply(tr);

        Assert.That(attr.Tags, Is.EqualTo(["", "", ""]));
        Assert.That(tr.labels, Is.Empty);
    }
}