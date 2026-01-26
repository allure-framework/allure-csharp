using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class LinkAttributeTests
{
    [Test]
    public void UrlOnlyLinkCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureLinkAttribute("foo").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.links,
            Is.EquivalentTo([new Link { url = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void LinkWithUrlAndTitleCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureLinkAttribute("foo")
        {
            Title = "bar",
        }.Apply(ctx);

        Assert.That(
            ctx.CurrentTest.links,
            Is.EquivalentTo([new Link { url = "foo", name = "bar" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void LinkWithUrlTitleAndTypeCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureLinkAttribute("foo")
        {
            Title = "bar",
            Type = "baz",
        }.Apply(ctx);

        Assert.That(
            ctx.CurrentTest.links,
            Is.EquivalentTo([new Link { url = "foo", name = "bar", type = "baz" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureLinkAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}
