using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AttributeTests;

class LabelAttributeTests
{
    [AllureLabel("foo", "bar")]
    class TargetBase { }

    [AllureLabel("baz", "qux")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CanBeQueriedFromBaseAndInheritedClasses()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureLabelAttribute>();

        Assert.That(typeAttributes, Has.Exactly(2).Items);
    }

    [Test]
    public void ItAddsLabelToTest()
    {
        TestResult tr = new();
        var attr = new AllureLabelAttribute("foo", "bar");

        attr.Apply(tr);

        Assert.That(attr.Name, Is.EqualTo("foo"));
        Assert.That(attr.Value, Is.EqualTo("bar"));
        Assert.That(
            tr.labels,
            Is.EquivalentTo([new Label { name = "foo", value = "bar" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void DoesNothingIfNameIsNull()
    {
        TestResult tr = new();

        new AllureLabelAttribute(null, "foo").Apply(tr);

        Assert.That(tr.labels, Is.Empty);
    }

    [Test]
    public void DoesNothingIfNameIsNullEmpty()
    {
        TestResult tr = new();

        new AllureLabelAttribute("", "foo").Apply(tr);

        Assert.That(tr.labels, Is.Empty);
    }

    [Test]
    public void DoesNothingIfValueIsNull()
    {
        TestResult tr = new();

        new AllureLabelAttribute("foo", null).Apply(tr);

        Assert.That(tr.labels, Is.Empty);
    }
}