using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class ParentSuiteAttributeTests
{
    [AllureParentSuite("foo")]
    class TargetBase { }

    [AllureParentSuite("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CanBeQueriedFromBaseAndInheritedClasses()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureParentSuiteAttribute>();

        Assert.That(typeAttributes, Has.Exactly(2).Items);
    }

    [Test]
    public void ParentSuiteCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureParentSuiteAttribute("foo");

        attr.Apply(tr);

        Assert.That(attr.Name, Is.EqualTo("parentSuite"));
        Assert.That(attr.Value, Is.EqualTo("foo"));
        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "parentSuite", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}