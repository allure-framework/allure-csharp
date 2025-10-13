using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserApiTests.NoContextTests;

class AllureApiNoContextTests
{
    static bool HasStep => AllureLifecycle.Instance.Context.HasStep;

    [Test]
    public void SetTestNameShouldNotThrow()
    {
        Assert.That(() => AllureApi.SetTestName("foo"), Throws.Nothing);
    }

    [Test]
    public void SetFixtureNameShouldNotThrow()
    {
        Assert.That(() => AllureApi.SetFixtureName("foo"), Throws.Nothing);
    }

    [Test]
    public void SetStepNameShouldNotThrow()
    {
        Assert.That(() => AllureApi.SetStepName("foo"), Throws.Nothing);
    }

    [Test]
    public void SetDescriptionShouldNotThrow()
    {
        Assert.That(() => AllureApi.SetDescription("foo"), Throws.Nothing);
    }

    [Test]
    public void SetDescriptionHtmlShouldNotThrow()
    {
        Assert.That(() => AllureApi.SetDescriptionHtml("foo"), Throws.Nothing);
    }

    [Test]
    public void AddLabelsShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddLabels(new Label()), Throws.Nothing);
    }

    [Test]
    public void AddLabelShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddLabel("foo", "bar"), Throws.Nothing);
        Assert.That(() => AllureApi.AddLabel(new Label()), Throws.Nothing);
    }

    [Test]
    public void SetSeverityShouldNotThrow()
    {
        Assert.That(() => AllureApi.SetSeverity(default), Throws.Nothing);
    }

    [Test]
    public void SetOwnerShouldNotThrow()
    {
        Assert.That(() => AllureApi.SetOwner("foo"), Throws.Nothing);
    }

    [Test]
    public void SetAllureIdShouldNotThrow()
    {
        Assert.That(() => AllureApi.SetAllureId(0), Throws.Nothing);
    }

    [Test]
    public void AddTagsShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddTags("foo"), Throws.Nothing);
    }

    [Test]
    public void AddParentSuiteShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddParentSuite("foo"), Throws.Nothing);
    }

    [Test]
    public void AddSuiteShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddSuite("foo"), Throws.Nothing);
    }

    [Test]
    public void AddSubSuiteShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddSubSuite("foo"), Throws.Nothing);
    }

    [Test]
    public void AddEpicShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddEpic("foo"), Throws.Nothing);
    }

    [Test]
    public void AddFeatureShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddFeature("foo"), Throws.Nothing);
    }

    [Test]
    public void AddStoryShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddStory("foo"), Throws.Nothing);
    }

    [Test]
    public void AddLinkShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddLink("foo"), Throws.Nothing);
        Assert.That(() => AllureApi.AddLink("foo", "bar"), Throws.Nothing);
        Assert.That(() => AllureApi.AddLink("foo", "bar", "baz"), Throws.Nothing);
    }

    [Test]
    public void AddLinksShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddLinks(new Link()), Throws.Nothing);
    }

    [Test]
    public void AddIssueShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddIssue("foo"), Throws.Nothing);
        Assert.That(() => AllureApi.AddIssue("foo", "bar"), Throws.Nothing);
    }

    [Test]
    public void AddTmsItemShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddTmsItem("foo"), Throws.Nothing);
        Assert.That(() => AllureApi.AddTmsItem("foo", "bar"), Throws.Nothing);
    }

    [Test]
    public void StepShouldDoNothing()
    {
        Assert.That(() => AllureApi.Step("foo"), Throws.Nothing);
        Assert.That(() => AllureApi.Step("foo", () => { }), Throws.Nothing);
        Assert.That(() => AllureApi.Step("foo", () => 0), Throws.Nothing);
        Assert.That(
            async () => await AllureApi.Step(
                "foo",
                async () => await Task.CompletedTask
            ),
            Throws.Nothing
        );
        Assert.That(
            async () => await AllureApi.Step(
                "foo",
                async () => await Task.FromResult(0)
            ),
            Throws.Nothing
        );
        Assert.That(HasStep, Is.False);
    }

    [Test]
    public void StepShouldCallAction()
    {
        bool called = false;

        AllureApi.Step("foo", () => { called = true; });

        Assert.That(called, Is.True);
    }

    [Test]
    public void StepShouldCallFunction()
    {
        Assert.That(AllureApi.Step("foo", () => "bar"), Is.EqualTo("bar"));
    }

    [Test]
    public async Task StepShouldCallAsyncAction()
    {
        bool called = false;

        await AllureApi.Step(
            "foo",
            async () => { called = true; await Task.CompletedTask; }
        );

        Assert.That(called, Is.True);
    }

    [Test]
    public async Task StepShouldCallAsyncFunction()
    {
        Assert.That(
            await AllureApi.Step(
                "foo",
                async () => await Task.FromResult("bar")
            ),
            Is.EqualTo("bar")
        );
    }

    [Test]
    public void AddAttachmentShouldNotThrowEvenIfNoFileExist()
    {
        Assert.That(() => AllureApi.AddAttachment("foo"), Throws.Nothing);
        Assert.That(() => AllureApi.AddAttachment("foo", "bar"), Throws.Nothing);
        Assert.That(() => AllureApi.AddAttachment("foo", "bar", "baz"), Throws.Nothing);
        Assert.That(
            () => AllureApi.AddAttachment("foo", "bar", [], "baz"),
            Throws.Nothing
        );
    }

    [Test]
    public void AddScreenDiffShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddScreenDiff("foo", "bar", "baz"), Throws.Nothing);
        Assert.That(() => AllureApi.AddScreenDiff([], [], []), Throws.Nothing);
    }

    [Test]
    public void AddTestParameterShouldNotThrow()
    {
        Assert.That(() => AllureApi.AddTestParameter("foo", default), Throws.Nothing);
        Assert.That(() => AllureApi.AddTestParameter("foo", default, default(ParameterMode)), Throws.Nothing);
        Assert.That(() => AllureApi.AddTestParameter("foo", default, default(bool)), Throws.Nothing);
        Assert.That(() => AllureApi.AddTestParameter("foo", default, default, default), Throws.Nothing);
        Assert.That(() => AllureApi.AddTestParameter(new Parameter()), Throws.Nothing);
    }

    [Test]
    public void AddTestParameterDoesNotTryToFormatValue()
    {
        bool called = false;
        Lazy<int> value = new(() => { called = true; return default; });

        AllureApi.AddTestParameter("foo", value);
        AllureApi.AddTestParameter("foo", value, default(bool));
        AllureApi.AddTestParameter("foo", value, default(ParameterMode));
        AllureApi.AddTestParameter("foo", value, default, default);

        Assert.That(called, Is.False);
    }
}
