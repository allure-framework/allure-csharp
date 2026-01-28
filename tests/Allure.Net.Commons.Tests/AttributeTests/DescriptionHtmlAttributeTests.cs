using System.Reflection;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Sdk;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class DescriptionHtmlAttributeTests
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

        new AllureDescriptionHtmlAttribute("foo").Apply(tr);

        Assert.That(tr.descriptionHtml, Is.EqualTo("foo"));
    }

    [Test]
    public void AppendsTestDescription()
    {
        TestResult tr = new() { descriptionHtml = "<p>foo</p>" };

        new AllureDescriptionHtmlAttribute("<p>bar</p>")
        {
            Append = true,
        }.Apply(tr);

        Assert.That(tr.descriptionHtml, Is.EqualTo("<p>foo</p><p>bar</p>"));
    }

    [Test]
    public void NoThrowIfCurrentValueIsNull()
    {
        TestResult tr = new();

        new AllureDescriptionHtmlAttribute("foo")
        {
            Append = true,
        }.Apply(tr);

        Assert.That(tr.descriptionHtml, Is.EqualTo("foo"));
    }

    [Test]
    public void DoesNothingIfValueIsNull()
    {
        TestResult tr = new() { descriptionHtml = "foo" };

        new AllureDescriptionHtmlAttribute(null).Apply(tr);

        Assert.That(tr.descriptionHtml, Is.EqualTo("foo"));
    }
}