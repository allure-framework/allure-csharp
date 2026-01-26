using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class TmsItemAttributeTests
{
    [Test]
    public void UrlOnlyTmsItemCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureTmsItemAttribute("foo").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.links,
            Is.EquivalentTo([new Link { url = "foo", type = "tms" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void TmsItemWithTitleCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureTmsItemAttribute("foo")
        {
            Title = "bar",
        }.Apply(ctx);

        Assert.That(
            ctx.CurrentTest.links,
            Is.EquivalentTo([new Link { url = "foo", name = "bar", type = "tms" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureTmsItemAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}
