using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class DescriptionAttributeTests
{
    [Test]
    public void SetsTestDescription()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureDescriptionAttribute("foo").Apply(ctx);

        Assert.That(ctx.CurrentTest.description, Is.EqualTo("foo"));
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureDescriptionAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}