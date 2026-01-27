using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class IssueAttributeTests
{
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
}
