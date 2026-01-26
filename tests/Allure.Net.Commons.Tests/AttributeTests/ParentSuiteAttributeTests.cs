using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class ParentSuiteAttributeTests
{
    [Test]
    public void ParentSuiteCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureParentSuiteAttribute("foo").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([new Label { name = "parentSuite", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureParentSuiteAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}