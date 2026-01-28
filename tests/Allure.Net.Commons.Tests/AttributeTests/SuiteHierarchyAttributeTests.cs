using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

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

        new AllureSuiteHierarchyAttribute("foo").Apply(tr);

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

        new AllureSuiteHierarchyAttribute("foo", "bar").Apply(tr);

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

        new AllureSuiteHierarchyAttribute("foo", "bar", "baz").Apply(tr);

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