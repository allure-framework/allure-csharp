using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserApiTests.NoContextTests;

class ExtendedApiNoContextTests
{
    static bool HasFixture => AllureLifecycle.Instance.Context.HasFixture;
    static bool HasStep => AllureLifecycle.Instance.Context.HasStep;

    class DummyException : Exception
    {
        public override string Message => throw new NotImplementedException();
    }

    [Test]
    public void StartBeforeFixtureShouldDoNothing()
    {
        ExtendedApi.StartBeforeFixture("foo");

        Assert.That(HasFixture, Is.False);
    }

    [Test]
    public void StartAfterFixtureShouldNotThrow()
    {
        ExtendedApi.StartAfterFixture("foo");

        Assert.That(HasFixture, Is.False);
    }

    [Test]
    public void PassFixtureShouldDoNothing()
    {
        bool called = false;
        void action(FixtureResult _) { called = true; }

        Assert.That(() => ExtendedApi.PassFixture(), Throws.Nothing);
        Assert.That(() => ExtendedApi.PassFixture(action), Throws.Nothing);
        Assert.That(called, Is.False);
    }

    [Test]
    public void FailFixtureShouldDoNothing()
    {
        bool called = false;
        void action(FixtureResult _) { called = true; }

        Assert.That(() => ExtendedApi.FailFixture(), Throws.Nothing);
        Assert.That(() => ExtendedApi.FailFixture(action), Throws.Nothing);
        Assert.That(() => ExtendedApi.FailFixture(new DummyException()), Throws.Nothing);
        Assert.That(() => ExtendedApi.FailFixture(action, new DummyException()), Throws.Nothing);
        Assert.That(called, Is.False);
    }

    [Test]
    public void BreakFixtureShouldDoNothing()
    {
        bool called = false;
        void action(FixtureResult _) { called = true; }

        Assert.That(() => ExtendedApi.BreakFixture(), Throws.Nothing);
        Assert.That(() => ExtendedApi.BreakFixture(action), Throws.Nothing);
        Assert.That(() => ExtendedApi.BreakFixture(new DummyException()), Throws.Nothing);
        Assert.That(() => ExtendedApi.BreakFixture(action, new DummyException()), Throws.Nothing);
        Assert.That(called, Is.False);
    }

    [Test]
    public void SkipFixtureShouldDoNothing()
    {
        bool called = false;
        void action(FixtureResult _) { called = true; }

        Assert.That(() => ExtendedApi.SkipFixture(), Throws.Nothing);
        Assert.That(() => ExtendedApi.SkipFixture(action), Throws.Nothing);
        Assert.That(called, Is.False);
    }

    [Test]
    public void ResolveFixtureShouldNotThrow()
    {
        Assert.That(() => ExtendedApi.ResolveFixture(new DummyException()), Throws.Nothing);
    }

    [Test]
    public void StartStepShouldDoNothing()
    {
        bool called = false;
        void action(StepResult _) { called = true; }

        ExtendedApi.StartStep("foo");
        ExtendedApi.StartStep("foo", action);

        Assert.That(HasStep, Is.False);
        Assert.That(called, Is.False);
    }

    [Test]
    public void PassStepShouldDoNothing()
    {
        bool called = false;
        void action(StepResult _) { called = true; }

        Assert.That(() => ExtendedApi.PassStep(), Throws.Nothing);
        Assert.That(() => ExtendedApi.PassStep(action), Throws.Nothing);
        Assert.That(called, Is.False);
    }

    [Test]
    public void FailStepShouldDoNothing()
    {
        bool called = false;
        void action(StepResult _) { called = true; }


        Assert.That(() => ExtendedApi.FailStep(), Throws.Nothing);
        Assert.That(() => ExtendedApi.FailStep(action), Throws.Nothing);
        Assert.That(() => ExtendedApi.FailStep(new DummyException()), Throws.Nothing);
        Assert.That(() => ExtendedApi.FailStep(action, new DummyException()), Throws.Nothing);
        Assert.That(called, Is.False);
    }

    [Test]
    public void BreakStepShouldDoNothing()
    {
        bool called = false;
        void action(StepResult _) { called = true; }

        Assert.That(() => ExtendedApi.BreakStep(), Throws.Nothing);
        Assert.That(() => ExtendedApi.BreakStep(action), Throws.Nothing);
        Assert.That(() => ExtendedApi.BreakStep(new DummyException()), Throws.Nothing);
        Assert.That(() => ExtendedApi.BreakStep(action, new DummyException()), Throws.Nothing);
        Assert.That(called, Is.False);
    }

    [Test]
    public void SkipStepShouldDoNothing()
    {
        bool called = false;
        void action(StepResult _) { called = true; }

        Assert.That(() => ExtendedApi.SkipStep(), Throws.Nothing);
        Assert.That(() => ExtendedApi.SkipStep(action), Throws.Nothing);
        Assert.That(called, Is.False);
    }

    [Test]
    public void ResolveStepShouldNotThrow()
    {
        Assert.That(() => ExtendedApi.ResolveStep(new DummyException()), Throws.Nothing);
    }

    [Test]
    public void BeforeShouldNotChangeContext()
    {
        Assert.That(() => ExtendedApi.Before("foo", () => { }), Throws.Nothing);
        Assert.That(() => ExtendedApi.Before("foo", () => 0), Throws.Nothing);
        Assert.That(
            async () => await ExtendedApi.Before(
                "foo",
                async () => await Task.CompletedTask
            ),
            Throws.Nothing
        );
        Assert.That(
            async () => await ExtendedApi.Before(
                "foo",
                async () => await Task.FromResult(0)
            ),
            Throws.Nothing
        );
        Assert.That(HasFixture, Is.False);
    }

    [Test]
    public void BeforeShouldCallAction()
    {
        bool called = false;

        ExtendedApi.Before("foo", () => { called = true; });

        Assert.That(called, Is.True);
    }

    [Test]
    public void BeforeShouldCallFunction()
    {
        Assert.That(ExtendedApi.Before("foo", () => "bar"), Is.EqualTo("bar"));
    }

    [Test]
    public async Task BeforeShouldCallAsyncAction()
    {
        bool called = false;

        await ExtendedApi.Before(
            "foo",
            async () => { called = true; await Task.CompletedTask; }
        );

        Assert.That(called, Is.True);
    }

    [Test]
    public async Task BeforeShouldCallAsyncFunction()
    {
        Assert.That(
            await ExtendedApi.Before(
                "foo",
                async () => await Task.FromResult("bar")
            ),
            Is.EqualTo("bar")
        );
    }

    [Test]
    public void AfterShouldNotChangeContext()
    {
        Assert.That(() => ExtendedApi.After("foo", () => { }), Throws.Nothing);
        Assert.That(() => ExtendedApi.After("foo", () => 0), Throws.Nothing);
        Assert.That(
            async () => await ExtendedApi.After(
                "foo",
                async () => await Task.CompletedTask
            ),
            Throws.Nothing
        );
        Assert.That(
            async () => await ExtendedApi.After(
                "foo",
                async () => await Task.FromResult(0)
            ),
            Throws.Nothing
        );
        Assert.That(HasFixture, Is.False);
    }

    [Test]
    public void AfterShouldCallAction()
    {
        bool called = false;

        ExtendedApi.After("foo", () => { called = true; });

        Assert.That(called, Is.True);
    }

    [Test]
    public void AfterShouldCallFunction()
    {
        Assert.That(ExtendedApi.After("foo", () => "bar"), Is.EqualTo("bar"));
    }

    [Test]
    public async Task AfterShouldCallAsyncAction()
    {
        bool called = false;

        await ExtendedApi.After(
            "foo",
            async () => { called = true; await Task.CompletedTask; }
        );

        Assert.That(called, Is.True);
    }

    [Test]
    public async Task AfterShouldCallAsyncFunction()
    {
        Assert.That(
            await ExtendedApi.After(
                "foo",
                async () => await Task.FromResult("bar")
            ),
            Is.EqualTo("bar")
        );
    }
}
