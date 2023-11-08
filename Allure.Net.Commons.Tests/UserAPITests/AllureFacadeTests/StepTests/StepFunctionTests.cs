using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests.StepTests;

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
        this.lifecycle = new AllureLifecycle();
        AllureApi.CurrentLifecycle = lifecycle;
    }

    [Test]
    public void ActionCanBeConvertedToBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        AllureApi.Before("My fixture", () => { });

        this.AssertBeforeFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void ActionCanBeConvertedToFailedBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => AllureApi.Before("My fixture", errorAction),
            Throws.Exception
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public void FuncCanBeConvertedToBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        var result = AllureApi.Before("My fixture", () => 0);

        Assert.That(result, Is.Zero);
        this.AssertBeforeFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void FuncCanBeConvertedToFailedBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => AllureApi.Before("My fixture", errorFunc),
            Throws.Exception
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public async Task AsyncActionCanBeConvertedToBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        await AllureApi.Before(
            "My fixture",
            async () => await Task.CompletedTask
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void AsyncActionCanBeConvertedToFailedBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            async () => await AllureApi.Before(
                "My fixture",
                asyncErrorAction
            ),
            Throws.Exception
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public async Task AsyncFuncCanBeConvertedToBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        await AllureApi.Before(
            "My fixture",
            async () => await Task.FromResult(0)
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void AsyncFuncCanBeConvertedToFailedBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            async () => await AllureApi.Before(
                "My fixture",
                asyncErrorFunc
            ),
            Throws.Exception
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public void ActionCanBeConvertedToAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        AllureApi.After("My fixture", () => { });

        this.AssertAfterFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void ActionCanBeConvertedToFailedAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => AllureApi.After("My fixture", errorAction),
            Throws.Exception
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public void FuncCanBeConvertedToAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        var result = AllureApi.After("My fixture", () => 0);

        Assert.That(result, Is.Zero);
        this.AssertAfterFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void FuncCanBeConvertedToFailedAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => AllureApi.After("My fixture", errorFunc),
            Throws.Exception
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public async Task AsyncActionCanBeConvertedToAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        await AllureApi.After(
            "My fixture",
            async () => await Task.CompletedTask
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void AsyncActionCanBeConvertedToFailedAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            async () => await AllureApi.After(
                "My fixture",
                asyncErrorAction
            ),
            Throws.Exception
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public async Task AsyncFuncCanBeConvertedToAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        await AllureApi.After(
            "My fixture",
            async () => await Task.FromResult(0)
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void AsyncFuncCanBeConvertedToFailedAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            async () => await AllureApi.After(
                "My fixture",
                asyncErrorFunc
            ),
            Throws.Exception
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public void StepWithNoActionCanBeCreated()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.Step("My step");

        this.AssertStepCompleted("My step", Status.passed);
    }

    [Test]
    public void ActionCanBeConvertedToStep()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        AllureApi.Step("My step", () => { });

        this.AssertStepCompleted("My step", Status.passed);
    }

    [Test]
    public void ActionCanBeConvertedToFailedStep()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            () => AllureApi.Step("My step", errorAction),
            Throws.Exception
        );

        this.AssertStepCompleted("My step", Status.failed);
    }

    [Test]
    public void FuncCanBeConvertedToStep()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        var result = AllureApi.Step("My step", () => 0);

        Assert.That(result, Is.Zero);
        this.AssertStepCompleted("My step", Status.passed);
    }

    [Test]
    public void FuncCanBeConvertedToFailedStep()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            () => AllureApi.Step("My step", errorFunc),
            Throws.Exception
        );

        this.AssertStepCompleted("My step", Status.failed);
    }

    [Test]
    public async Task AsyncActionCanBeConvertedToStep()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        await AllureApi.Step(
            "My step",
            async () => await Task.CompletedTask
        );

        this.AssertStepCompleted("My step", Status.passed);
    }

    [Test]
    public void AsyncActionCanBeConvertedToFailedStep()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            async () => await AllureApi.Step("My step", asyncErrorAction),
            Throws.Exception
        );

        this.AssertStepCompleted("My step", Status.failed);
    }

    [Test]
    public async Task AsyncFuncCanBeConvertedToStep()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        await AllureApi.Step(
            "My step",
            async () => await Task.FromResult(0)
        );

        this.AssertStepCompleted("My step", Status.passed);
    }

    [Test]
    public void AsyncFuncCanBeConvertedToFailedStep()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            async () => await AllureApi.Step("My step", asyncErrorFunc),
            Throws.Exception
        );

        this.AssertStepCompleted("My step", Status.failed);
    }

    void AssertBeforeFixtureCompleted(string name, Status status) =>
        this.AssertFixtureCompleted(tc => tc.befores, name, status);

    void AssertAfterFixtureCompleted(string name, Status status) =>
        this.AssertFixtureCompleted(tc => tc.afters, name, status);

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
