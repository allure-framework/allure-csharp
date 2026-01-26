using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class StoryAttributeTests
{
    [Test]
    public void StoryCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureStoryAttribute("foo").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([new Label { name = "story", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureStoryAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}
