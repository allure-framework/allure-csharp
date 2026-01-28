using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class EpicAttributeTests
{
    [AllureEpic("foo")]
    class TargetBase { }

    [AllureEpic("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CanBeQueriedFromBaseAndInheritedClasses()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureEpicAttribute>();

        Assert.That(typeAttributes, Has.Exactly(2).Items);
    }

    [Test]
    public void EpicCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureEpicAttribute("foo").Apply(tr);

        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "epic", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}