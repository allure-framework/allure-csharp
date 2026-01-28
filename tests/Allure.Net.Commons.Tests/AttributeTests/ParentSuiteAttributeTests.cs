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

        new AllureParentSuiteAttribute("foo").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "parentSuite", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}