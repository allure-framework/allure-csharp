using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class IssueAttributeTests
{
    [Test]
    public void UrlOnlyIssueCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureIssueAttribute("foo").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.links,
            Is.EquivalentTo([new Link { url = "foo", type = "issue" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void IssueWithTitleCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureIssueAttribute("foo")
        {
            Title = "bar",
        }.Apply(ctx);

        Assert.That(
            ctx.CurrentTest.links,
            Is.EquivalentTo([new Link { url = "foo", name = "bar", type = "issue" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureIssueAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}
