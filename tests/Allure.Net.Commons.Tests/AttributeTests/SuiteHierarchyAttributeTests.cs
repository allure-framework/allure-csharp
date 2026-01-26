using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.AttributeTests;

class SuiteHierarchyAttributeTests
{
    [Test]
    public void SingleSuiteCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureSuiteHierarchyAttribute("foo").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([new Label { name = "suite", value = "foo" }])
                .UsingPropertiesComparer()
        );
    }

    [Test]
    public void TwoLevelSuiteHierarchyCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureSuiteHierarchyAttribute("foo", "bar").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([
                new Label { name = "parentSuite", value = "foo" },
                new Label { name = "suite", value = "bar" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void ThreeLevelSuiteHierarchyCanBeAddedToTest()
    {
        var ctx = new AllureContext().WithTestContext(new());

        new AllureSuiteHierarchyAttribute("foo", "bar", "baz").Apply(ctx);

        Assert.That(
            ctx.CurrentTest.labels,
            Is.EquivalentTo([
                new Label { name = "parentSuite", value = "foo" },
                new Label { name = "suite", value = "bar" },
                new Label { name = "subSuite", value = "baz" },
            ]).UsingPropertiesComparer()
        );
    }

    [Test]
    public void NoEffectIfNoTestIsRunning()
    {
        var ctx = new AllureContext();
        var attr = new AllureSuiteHierarchyAttribute("foo");

        Assert.That(() => attr.Apply(ctx), Throws.Nothing);
    }
}