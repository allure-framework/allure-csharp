using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;


class MetaAttributeTests
{
    [AllureTag("foo")]
    [AllureSuite("bar")]
    [AllureIssue("foo", Title = "bar")]
    class FooAttribute : AllureMetaAttribute { }

    [Test]
    public void AttributesOfTheMetaAttributeAreApplied()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new FooAttribute().Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([
                new Label { name = "tag", value = "foo" },
                new Label { name = "suite", value = "bar" },
            ]).UsingPropertiesComparer()
        );
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
        var attr = new FooAttribute();

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}
