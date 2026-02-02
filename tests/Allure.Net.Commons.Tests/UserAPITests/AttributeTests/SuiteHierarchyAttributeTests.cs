using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AttributeTests;

class SuiteHierarchyAttributeTests
{
    [AllureSuiteHierarchy("foo")]
    class TargetBase { }

    [AllureSuiteHierarchy("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CantBeInherited()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureSuiteHierarchyAttribute>();

        Assert.That(typeAttributes, Has.Exactly(1).Items);
    }

    [Test]
    public void SingleSuiteCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureSuiteHierarchyAttribute("foo");

        attr.Apply(tr);

        Assert.That(attr.ParentSuite, Is.Null);
        Assert.That(attr.Suite, Is.EqualTo("foo"));
        Assert.That(attr.SubSuite, Is.Null);
        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "suite", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void TwoLevelSuiteHierarchyCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureSuiteHierarchyAttribute("foo", "bar");

        attr.Apply(tr);

        Assert.That(attr.ParentSuite, Is.EqualTo("foo"));
        Assert.That(attr.Suite, Is.EqualTo("bar"));
        Assert.That(attr.SubSuite, Is.Null);
        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "parentSuite", value = "foo" },
                new Label { name = "suite", value = "bar" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void ThreeLevelSuiteHierarchyCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureSuiteHierarchyAttribute("foo", "bar", "baz");

        attr.Apply(tr);

        Assert.That(attr.ParentSuite, Is.EqualTo("foo"));
        Assert.That(attr.Suite, Is.EqualTo("bar"));
        Assert.That(attr.SubSuite, Is.EqualTo("baz"));
        Assert.That(
            tr.labels,
            Is.EquivalentTo([
                new Label { name = "parentSuite", value = "foo" },
                new Label { name = "suite", value = "bar" },
                new Label { name = "subSuite", value = "baz" },
            ]).UsingPropertiesComparer()
        );
    }
}