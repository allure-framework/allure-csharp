using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class SuiteAttributeTests
{
    [Test]
    public void SuiteCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureSuiteAttribute("foo").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([new Label { name = "suite", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureSuiteAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}