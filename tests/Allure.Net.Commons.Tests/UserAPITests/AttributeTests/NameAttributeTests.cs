using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AttributeTests;

class NameAttributeTests
{
    [AllureName("foo")]
    class TargetBase { }

    [AllureName("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CantBeInherited()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureNameAttribute>();

        Assert.That(typeAttributes, Has.Exactly(1).Items);
    }

    [Test]
    public void ItSetsTestName()
    {
        TestResult tr = new();

        new AllureNameAttribute("foo").Apply(tr);

        Assert.That(tr.name, Is.EqualTo("foo"));
    }

    [Test]
    public void DoesNothingIfNameIsNull()
    {
        TestResult tr = new() { name = "foo" };

        new AllureNameAttribute(null).Apply(tr);

        Assert.That(tr.name, Is.EqualTo("foo"));
    }
}