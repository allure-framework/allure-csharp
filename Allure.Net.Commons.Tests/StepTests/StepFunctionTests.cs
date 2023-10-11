using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Allure.Net.Commons.Steps;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.StepTests;

class StepAndFixtureFunctionTests
{
    AllureLifecycle lifecycle;

    static readonly Action errorAction = () => throw new Exception();
    static readonly Func<int> errorFunc = () => throw new Exception();
    static readonly Func<Task> asyncErrorAction
        = async () => await Task.FromException(new Exception());
    static readonly Func<Task<int>> asyncErrorFunc
        = async () => await Task.FromException<int>(new Exception());

    [SetUp]
    public void SetUp()
    {
        lifecycle = new AllureLifecycle();
        CoreStepsHelper.CurrentLifecycle = lifecycle;
    }

    [Test]
    public void ActionCanBeConvertedToBeforeFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        CoreStepsHelper.Before("My fixture", () => { });

        AssertBeforeFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void ActionCanBeConvertedToFailedBeforeFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => CoreStepsHelper.Before("My fixture", errorAction),
            Throws.Exception
        );

        AssertBeforeFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public void FuncCanBeConvertedToBeforeFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        var result = CoreStepsHelper.Before("My fixture", () => 0);

        Assert.That(result, Is.Zero);
        AssertBeforeFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void FuncCanBeConvertedToFailedBeforeFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => CoreStepsHelper.Before("My fixture", errorFunc),
            Throws.Exception
        );

        AssertBeforeFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public async Task AsyncActionCanBeConvertedToBeforeFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        await CoreStepsHelper.Before(
            "My fixture",
            async () => await Task.CompletedTask
        );

        AssertBeforeFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void AsyncActionCanBeConvertedToFailedBeforeFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            async () => await CoreStepsHelper.Before(
                "My fixture",
                asyncErrorAction
            ),
            Throws.Exception
        );

        AssertBeforeFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public async Task AsyncFuncCanBeConvertedToBeforeFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        await CoreStepsHelper.Before(
            "My fixture",
            async () => await Task.FromResult(0)
        );

        AssertBeforeFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void AsyncFuncCanBeConvertedToFailedBeforeFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            async () => await CoreStepsHelper.Before(
                "My fixture",
                asyncErrorFunc
            ),
            Throws.Exception
        );

        AssertBeforeFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public void ActionCanBeConvertedToAfterFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        CoreStepsHelper.After("My fixture", () => { });

        AssertAfterFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void ActionCanBeConvertedToFailedAfterFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => CoreStepsHelper.After("My fixture", errorAction),
            Throws.Exception
        );

        AssertAfterFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public void FuncCanBeConvertedToAfterFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        var result = CoreStepsHelper.After("My fixture", () => 0);

        Assert.That(result, Is.Zero);
        AssertAfterFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void FuncCanBeConvertedToFailedAfterFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => CoreStepsHelper.After("My fixture", errorFunc),
            Throws.Exception
        );

        AssertAfterFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public async Task AsyncActionCanBeConvertedToAfterFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        await CoreStepsHelper.After(
            "My fixture",
            async () => await Task.CompletedTask
        );

        AssertAfterFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void AsyncActionCanBeConvertedToFailedAfterFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            async () => await CoreStepsHelper.After(
                "My fixture",
                asyncErrorAction
            ),
            Throws.Exception
        );

        AssertAfterFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public async Task AsyncFuncCanBeConvertedToAfterFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        await CoreStepsHelper.After(
            "My fixture",
            async () => await Task.FromResult(0)
        );

        AssertAfterFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void AsyncFuncCanBeConvertedToFailedAfterFixture()
    {
        lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            async () => await CoreStepsHelper.After(
                "My fixture",
                asyncErrorFunc
            ),
            Throws.Exception
        );

        AssertAfterFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public void StepWithNoActionCanBeCreated()
    {
        lifecycle.StartTestCase(new() { uuid = "uuid" });

        CoreStepsHelper.Step("My step");

        AssertStepCompleted("My step", Status.passed);
    }

    [Test]
    public void ActionCanBeConvertedToStep()
    {
        lifecycle.StartTestCase(new() { uuid = "uuid" });

        CoreStepsHelper.Step("My step", () => { });

        AssertStepCompleted("My step", Status.passed);
    }

    [Test]
    public void ActionCanBeConvertedToFailedStep()
    {
        lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            () => CoreStepsHelper.Step("My step", errorAction),
            Throws.Exception
        );

        AssertStepCompleted("My step", Status.failed);
    }

    [Test]
    public void FuncCanBeConvertedToStep()
    {
        lifecycle.StartTestCase(new() { uuid = "uuid" });

        var result = CoreStepsHelper.Step("My step", () => 0);

        Assert.That(result, Is.Zero);
        AssertStepCompleted("My step", Status.passed);
    }

    [Test]
    public void FuncCanBeConvertedToFailedStep()
    {
        lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            () => CoreStepsHelper.Step("My step", errorFunc),
            Throws.Exception
        );

        AssertStepCompleted("My step", Status.failed);
    }

    [Test]
    public async Task AsyncActionCanBeConvertedToStep()
    {
        lifecycle.StartTestCase(new() { uuid = "uuid" });

        await CoreStepsHelper.Step(
            "My step",
            async () => await Task.CompletedTask
        );

        AssertStepCompleted("My step", Status.passed);
    }

    [Test]
    public void AsyncActionCanBeConvertedToFailedStep()
    {
        lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            async () => await CoreStepsHelper.Step("My step", asyncErrorAction),
            Throws.Exception
        );

        AssertStepCompleted("My step", Status.failed);
    }

    [Test]
    public async Task AsyncFuncCanBeConvertedToStep()
    {
        lifecycle.StartTestCase(new() { uuid = "uuid" });

        await CoreStepsHelper.Step(
            "My step",
            async () => await Task.FromResult(0)
        );

        AssertStepCompleted("My step", Status.passed);
    }

    [Test]
    public void AsyncFuncCanBeConvertedToFailedStep()
    {
        lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            async () => await CoreStepsHelper.Step("My step", asyncErrorFunc),
            Throws.Exception
        );

        AssertStepCompleted("My step", Status.failed);
    }

    void AssertBeforeFixtureCompleted(string name, Status status) =>
        AssertFixtureCompleted(tc => tc.befores, name, status);

    void AssertAfterFixtureCompleted(string name, Status status) =>
        AssertFixtureCompleted(tc => tc.afters, name, status);

    void AssertFixtureCompleted(
        Func<TestResultContainer, List<FixtureResult>> getFixtures,
        string name,
        Status status
    )
    {
        Assert.That(lifecycle.Context.HasFixture, Is.False);
        Assert.That(lifecycle.Context.HasContainer);
        var fixtures = getFixtures(lifecycle.Context.CurrentContainer);
        Assert.That(fixtures, Has.Count.EqualTo(1));
        var fixture = fixtures.First();
        Assert.That(fixture.name, Is.EqualTo(name));
        Assert.That(fixture.status, Is.EqualTo(status));
    }

    void AssertStepCompleted(string name, Status status)
    {
        Assert.That(lifecycle.Context.HasStep, Is.False);
        Assert.That(lifecycle.Context.HasTest);
        var steps = lifecycle.Context.CurrentTest.steps;
        Assert.That(steps, Has.Count.EqualTo(1));
        var fixture = steps.First();
        Assert.That(fixture.name, Is.EqualTo(name));
        Assert.That(fixture.status, Is.EqualTo(status));
    }
}
