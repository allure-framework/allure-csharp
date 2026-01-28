using System.Reflection;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class IssueAttributeTests
{
    [AllureIssue("foo")]
    class TargetBase { }

    [AllureIssue("bar")]
    class TargetDerived : TargetBase { }

    [Test]
    public void CanBeQueriedFromBaseAndInheritedClasses()
    {
        var typeAttributes = typeof(TargetDerived).GetCustomAttributes<AllureIssueAttribute>();

        Assert.That(typeAttributes, Has.Exactly(2).Items);
    }

    [Test]
    public void UrlOnlyIssueCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureIssueAttribute("foo").Apply(tr);

        Assert.That(
            tr.links,
            Is.EquivalentTo([new Link { url = "foo", type = "issue" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void IssueWithTitleCanBeAddedToTest()
    {
        TestResult tr = new();

        new AllureIssueAttribute("foo")
        {
            Title = "bar",
        }.Apply(tr);

        Assert.That(
            tr.links,
            Is.EquivalentTo([new Link { url = "foo", name = "bar", type = "issue" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void DoesNothingIfUrlIsNull()
    {
        TestResult tr = new();

        new AllureIssueAttribute(null).Apply(tr);

        Assert.That(tr.links, Is.Empty);
    }
}
