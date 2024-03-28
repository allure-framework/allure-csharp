using System;
using NUnit.Framework;

#nullable enable

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests.StepTests;

internal class ExplicitFixtureAndStepTests : AllureApiTestFixture
{
    FixtureResult? fixture;

    [SetUp]
    public void SetFixtureContext()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "1" });
        ExtendedApi.StartBeforeFixture("Fixture");
        this.fixture = this.Context.FixtureContext;
    }

    [Test]
    public void TestSkipFixture()
    {
        ExtendedApi.SkipFixture();

        Assert.That(this.fixture!.status, Is.EqualTo(Status.skipped));
    }

    [Test]
    public void TestSkipFixtureWithCallback()
    {
        ExtendedApi.SkipFixture(s => s.description = "description");

        Assert.That(this.fixture!.status, Is.EqualTo(Status.skipped));
        Assert.That(this.fixture.description, Is.EqualTo("description"));
    }

    [Test]
    public void TestFailFixtureWithError()
    {
        var error = new Exception("message");
        ExtendedApi.FailFixture(error);

        this.AssertFixtureStatus(Status.failed, "message", "System.Exception");
    }

    [Test]
    public void TestFailFixtureWithErrorAndAction()
    {
        var error = new Exception("message");
        ExtendedApi.FailFixture(f => f.description = "description", error);

        this.AssertFixtureStatus(Status.failed, "message", "System.Exception");
        Assert.That(this.fixture!.description, Is.EqualTo("description"));
    }

    [Test]
    public void TestBreakFixtureWithError()
    {
        var error = new Exception("message");
        ExtendedApi.BreakFixture(error);

        this.AssertFixtureStatus(Status.broken, "message", "System.Exception");
    }

    [Test]
    public void TestBreakFixtureWithErrorAndAction()
    {
        var error = new Exception("message");
        ExtendedApi.BreakFixture(f => f.description = "description", error);

        this.AssertFixtureStatus(Status.broken, "message", "System.Exception");
        Assert.That(this.fixture!.description, Is.EqualTo("description"));
    }

    [Test]
    public void TestSkipStep()
    {
        ExtendedApi.StartStep("step");
        var step = this.Context.CurrentStep;

        ExtendedApi.SkipStep();

        Assert.That(step.status, Is.EqualTo(Status.skipped));
    }

    [Test]
    public void TestSkipStepWithCallback()
    {
        ExtendedApi.StartStep("step");
        var step = this.Context.CurrentStep;

        ExtendedApi.SkipStep(s => s.description = "description");

        Assert.That(step.status, Is.EqualTo(Status.skipped));
        Assert.That(step.description, Is.EqualTo("description"));
    }

    [Test]
    public void TestFailStepWithError()
    {
        var error = new Exception("message");
        ExtendedApi.StartStep("step");
        var step = this.Context.CurrentStep;

        ExtendedApi.FailStep(error);

        AssertStepStatus(step, Status.failed, "message", "System.Exception");
    }

    [Test]
    public void TestFailStepWithErrorAndAction()
    {
        var error = new Exception("message");
        ExtendedApi.StartStep("step");
        var step = this.Context.CurrentStep;

        ExtendedApi.FailStep(f => f.description = "description", error);

        AssertStepStatus(step, Status.failed, "message", "System.Exception");
        Assert.That(step.description, Is.EqualTo("description"));
    }

    [Test]
    public void TestBreakStepWithError()
    {
        var error = new Exception("message");
        ExtendedApi.StartStep("step");
        var step = this.Context.CurrentStep;

        ExtendedApi.BreakStep(error);

        AssertStepStatus(step, Status.broken, "message", "System.Exception");
    }

    [Test]
    public void TestBreakStepWithErrorAndAction()
    {
        var error = new Exception("message");
        ExtendedApi.StartStep("step");
        var step = this.Context.CurrentStep;

        ExtendedApi.BreakStep(f => f.description = "description", error);

        AssertStepStatus(step, Status.broken, "message", "System.Exception");
        Assert.That(step.description, Is.EqualTo("description"));
    }

    void AssertFixtureStatus(Status status, string message, string trace)
    {
        Assert.That(this.fixture!.status, Is.EqualTo(status));
        Assert.That(this.fixture.statusDetails.message, Is.EqualTo(message));
        Assert.That(this.fixture.statusDetails.trace, Contains.Substring(trace));
    }

    static void AssertStepStatus(StepResult step, Status status, string message, string trace)
    {
        Assert.That(step.status, Is.EqualTo(status));
        Assert.That(step.statusDetails.message, Is.EqualTo(message));
        Assert.That(step.statusDetails.trace, Contains.Substring(trace));
    }
}
