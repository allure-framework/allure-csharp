using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AttributeTests;

class SuiteAttributeTests
{
    [AllureSuite("foo")]
    class TargetBase { }

    [AllureSuite("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CanBeQueriedFromBaseAndInheritedClasses()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureSuiteAttribute>();

        Assert.That(typeAttributes, Has.Exactly(2).Items);
    }

    [Test]
    public void SuiteCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureSuiteAttribute("foo");

        attr.Apply(tr);

        Assert.That(attr.Name, Is.EqualTo("suite"));
        Assert.That(attr.Value, Is.EqualTo("foo"));
        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "suite", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}