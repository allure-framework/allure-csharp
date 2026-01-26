using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class FeatureAttributeTests
{
    [Test]
    public void FeatureCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureFeatureAttribute("foo").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([new Label { name = "feature", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureFeatureAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}
