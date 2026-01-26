using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class NameAttributeTests
{
    [Test]
    public void ItSetsTestName()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureNameAttribute("foo").Apply(ctx);

        Assert.That(ctx.CurrentTest.name, Is.EqualTo("foo"));
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureNameAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}