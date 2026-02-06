using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AttributeTests;

class BddHierarchyAttributeTests
{
    [AllureBddHierarchy("foo")]
    class TargetBase { }

    [AllureBddHierarchy("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CantBeInherited()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureBddHierarchyAttribute>();

        Assert.That(typeAttributes, Has.Exactly(1).Items);
    }

    [Test]
    public void SingleFeatureCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureBddHierarchyAttribute("foo");

        attr.Apply(tr);

        Assert.That(attr.Epic, Is.Null);
        Assert.That(attr.Feature, Is.EqualTo("foo"));
        Assert.That(attr.Story, Is.Null);
        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "feature", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void EpicAndFeatureCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureBddHierarchyAttribute("foo", "bar");

        attr.Apply(tr);

        Assert.That(attr.Epic, Is.EqualTo("foo"));
        Assert.That(attr.Feature, Is.EqualTo("bar"));
        Assert.That(attr.Story, Is.Null);
        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "epic", value = "foo" },
                new Label { name = "feature", value = "bar" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void EpicFeatureAndStoryCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureBddHierarchyAttribute("foo", "bar", "baz");

        attr.Apply(tr);

        Assert.That(attr.Epic, Is.EqualTo("foo"));
        Assert.That(attr.Feature, Is.EqualTo("bar"));
        Assert.That(attr.Story, Is.EqualTo("baz"));
        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "epic", value = "foo" },
                new Label { name = "feature", value = "bar" },
                new Label { name = "story", value = "baz" },
            ]).UsingPropertiesComparer()
        );
    }
}