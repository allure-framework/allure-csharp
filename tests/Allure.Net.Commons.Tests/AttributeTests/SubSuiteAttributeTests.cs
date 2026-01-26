using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class SubSuiteAttributeTests
{
    [Test]
    public void SubSuiteCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureSubSuiteAttribute("foo").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([new Label { name = "subSuite", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureSubSuiteAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}