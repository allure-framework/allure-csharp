using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class TagAttributeTests
{
    [Test]
    public void ItAddsSingleTagToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureTagAttribute("foo").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([new Label { name = "tag", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void ItAddsMultipleTagsToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureTagAttribute("foo", "bar", "baz").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([
                new Label { name = "tag", value = "foo" },
                new Label { name = "tag", value = "bar" },
                new Label { name = "tag", value = "baz" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureTagAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}