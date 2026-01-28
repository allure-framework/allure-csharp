using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class SubSuiteAttributeTests
{
    [AllureSubSuite("foo")]
    class TargetBase { }

    [AllureSubSuite("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CanBeQueriedFromBaseAndInheritedClasses()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureSubSuiteAttribute>();

        Assert.That(typeAttributes, Has.Exactly(2).Items);
    }

    [Test]
    public void SubSuiteCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureSubSuiteAttribute("foo").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "subSuite", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}