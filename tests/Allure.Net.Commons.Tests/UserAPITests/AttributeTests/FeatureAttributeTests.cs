using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AttributeTests;

class FeatureAttributeTests
{
    [AllureFeature("foo")]
    class TargetBase { }

    [AllureFeature("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CanBeQueriedFromBaseAndInheritedClasses()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureFeatureAttribute>();

        Assert.That(typeAttributes, Has.Exactly(2).Items);
    }

    [Test]
    public void FeatureCanBeAddedToTest()
    {
        TestResult tr = new();
        var attr = new AllureFeatureAttribute("foo");

        attr.Apply(tr);

        Assert.That(attr.Name, Is.EqualTo("feature"));
        Assert.That(attr.Value, Is.EqualTo("foo"));
        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "feature", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }
}
