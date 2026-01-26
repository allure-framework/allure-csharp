using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class LabelAttributeTests
{
    [Test]
    public void ItAddsLabelToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureLabelAttribute("foo", "bar").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([new Label { name = "foo", value = "bar" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureLabelAttribute("foo", "bar");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}