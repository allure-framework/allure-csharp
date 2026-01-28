using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

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

        new AllureBddHierarchyAttribute("foo").Apply(tr);

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

        new AllureBddHierarchyAttribute("foo", "bar").Apply(tr);

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

        new AllureBddHierarchyAttribute("foo", "bar", "baz").Apply(tr);

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