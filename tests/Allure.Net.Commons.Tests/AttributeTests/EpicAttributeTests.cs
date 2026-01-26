using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class EpicAttributeTests
{
    [Test]
    public void EpicCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureEpicAttribute("foo").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([new Label { name = "epic", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureEpicAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}