using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

#nullable enable

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests.StepTests;

class LambdaStepTests : LambdaApiTestFixture
{
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
            () => AllureApi.Step("My step", failAction),
            Throws.InstanceOf<FailException>()
        );

        this.AssertStepCompleted("My step", Status.failed, "message", typeof(FailException));
    }

    [Test]
    public void ActionCanBeConvertedToBrokenStep()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            () => AllureApi.Step("My step", breakAction),
            Throws.Exception
        );

        this.AssertStepCompleted("My step", Status.broken, "message", typeof(Exception));
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
            () => AllureApi.Step("My step", failFunc),
            Throws.InstanceOf<FailException>()
        );

        this.AssertStepCompleted("My step", Status.failed, "message", typeof(FailException));
    }

    [Test]
    public void FuncCanBeConvertedToBrokenStep()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            () => AllureApi.Step("My step", breakFunc),
            Throws.Exception
        );

        this.AssertStepCompleted("My step", Status.broken, "message", typeof(Exception));
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
            async () => await AllureApi.Step("My step", asyncFailAction),
            Throws.InstanceOf<FailException>()
        );

        this.AssertStepCompleted("My step", Status.failed, "message", typeof(FailException));
    }

    [Test]
    public void AsyncActionCanBeConvertedToBrokenStep()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            async () => await AllureApi.Step("My step", asyncBreakAction),
            Throws.Exception
        );

        this.AssertStepCompleted("My step", Status.broken, "message", typeof(Exception));
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
            async () => await AllureApi.Step("My step", asyncFailFunc),
            Throws.InstanceOf<FailException>()
        );

        this.AssertStepCompleted("My step", Status.failed, "message", typeof(FailException));
    }

    [Test]
    public void AsyncFuncCanBeConvertedToBrokenStep()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            async () => await AllureApi.Step("My step", asyncBreakFunc),
            Throws.Exception
        );

        this.AssertStepCompleted("My step", Status.broken, "message", typeof(Exception));
    }

    [Test]
    public void SubclassOfFailExceptionTreatedAsAssertionFailure()
    {
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            () => AllureApi.Step("My step", () => throw new InheritedFailException()),
            Throws.InstanceOf<InheritedFailException>()
        );

        this.AssertStepCompleted("My step", Status.failed, "message", typeof(InheritedFailException));
    }

    [Test]
    public void ImplementationOfFailInterfaceTreatedAsAssertionFailure()
    {
        this.lifecycle.AllureConfiguration.FailExceptions = new()
        {
            typeof(IErrorInterface).FullName
        };
        this.lifecycle.StartTestCase(new() { uuid = "uuid" });

        Assert.That(
            () => AllureApi.Step("My step", () => throw new InheritedFailException()),
            Throws.InstanceOf<InheritedFailException>()
        );

        this.AssertStepCompleted("My step", Status.failed, "message", typeof(InheritedFailException));
    }

    void AssertStepCompleted(string name, Status status, string? message = null, Type? exceptionType = null)
    {
        Assert.That(lifecycle.Context.HasStep, Is.False);
        Assert.That(lifecycle.Context.HasTest);
        var steps = lifecycle.Context.CurrentTest.steps;
        Assert.That(steps, Has.Count.EqualTo(1));
        var step = steps.First();
        Assert.That(step.name, Is.EqualTo(name));
        Assert.That(step.status, Is.EqualTo(status));
        Assert.That(step.statusDetails?.message, Is.EqualTo(message));
        if (message is not null)
        {
            Assert.That(step.statusDetails?.trace, Contains.Substring(message));
        }
        if (exceptionType?.FullName is not null)
        {
            Assert.That(step.statusDetails?.trace, Contains.Substring(exceptionType.FullName));
        }
    }
}
