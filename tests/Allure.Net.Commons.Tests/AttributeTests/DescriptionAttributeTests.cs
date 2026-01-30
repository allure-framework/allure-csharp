using System.Reflection;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Sdk;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class DescriptionAttributeTests
{
    [AllureDescription("foo")]
    class TargetBase { }

    [AllureDescription("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CanBeQueriedFromBaseAndInheritedClasses()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureDescriptionAttribute>();

        Assert.That(typeAttributes, Has.Exactly(2).Items);
    }

    [Test]
    public void SetsTestDescription()
    {
        TestResult tr = new();
        var attr = new AllureDescriptionAttribute("foo");

        attr.Apply(tr);

        Assert.That(attr.MarkdownText, Is.EqualTo("foo"));
        Assert.That(tr.description, Is.EqualTo("foo"));
    }

    [Test]
    public void AppendsTestDescription()
    {
        TestResult tr = new() { description = "foo" };

        new AllureDescriptionAttribute("bar")
        {
            Append = true,
        }.Apply(tr);

        Assert.That(tr.description, Is.EqualTo("foo\n\nbar"));
    }

    [Test]
    public void OmitsSeparatorWhenAppendingToNullDescription()
    {
        TestResult tr = new();

        new AllureDescriptionAttribute("foo")
        {
            Append = true,
        }.Apply(tr);

        Assert.That(tr.description, Is.EqualTo("foo"));
    }

    [Test]
    public void OmitsSeparatorWhenAppendingToEmptyDescription()
    {
        TestResult tr = new(){ description = "" };

        new AllureDescriptionAttribute("foo")
        {
            Append = true,
        }.Apply(tr);

        Assert.That(tr.description, Is.EqualTo("foo"));
    }

    [Test]
    public void DoesNothingIfDescriptionIsNull()
    {
        TestResult tr = new() { description = "foo" };

        new AllureDescriptionAttribute(null).Apply(tr);

        Assert.That(tr.description, Is.EqualTo("foo"));
    }
}