using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests.StepTests;

class LambdaStepTests : AllureApiTestFixture
{
    static readonly Action errorAction = () => throw new Exception();
    static readonly Func<int> errorFunc = () => throw new Exception();
    static readonly Func<Task> asyncErrorAction
        = async () => await Task.FromException(new Exception());
    static readonly Func<Task<int>> asyncErrorFunc
        = async () => await Task.FromException<int>(new Exception());

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
