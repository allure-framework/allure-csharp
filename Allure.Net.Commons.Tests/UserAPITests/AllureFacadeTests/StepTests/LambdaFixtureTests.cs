using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests.StepTests;

class LambdaFixtureTests : AllureApiTestFixture
{
    static readonly Action errorAction = () => throw new Exception();
    static readonly Func<int> errorFunc = () => throw new Exception();
    static readonly Func<Task> asyncErrorAction
        = async () => await Task.FromException(new Exception());
    static readonly Func<Task<int>> asyncErrorFunc
        = async () => await Task.FromException<int>(new Exception());

    [Test]
    public void ActionCanBeConvertedToBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        ExtendedApi.Before("My fixture", () => { });

        this.AssertBeforeFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void ActionCanBeConvertedToFailedBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => ExtendedApi.Before("My fixture", errorAction),
            Throws.Exception
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public void FuncCanBeConvertedToBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        var result = ExtendedApi.Before("My fixture", () => 0);

        Assert.That(result, Is.Zero);
        this.AssertBeforeFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void FuncCanBeConvertedToFailedBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => ExtendedApi.Before("My fixture", errorFunc),
            Throws.Exception
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public async Task AsyncActionCanBeConvertedToBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        await ExtendedApi.Before(
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
            async () => await ExtendedApi.Before(
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

        await ExtendedApi.Before(
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
            async () => await ExtendedApi.Before(
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

        ExtendedApi.After("My fixture", () => { });

        this.AssertAfterFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void ActionCanBeConvertedToFailedAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => ExtendedApi.After("My fixture", errorAction),
            Throws.Exception
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public void FuncCanBeConvertedToAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        var result = ExtendedApi.After("My fixture", () => 0);

        Assert.That(result, Is.Zero);
        this.AssertAfterFixtureCompleted("My fixture", Status.passed);
    }

    [Test]
    public void FuncCanBeConvertedToFailedAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => ExtendedApi.After("My fixture", errorFunc),
            Throws.Exception
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.failed);
    }

    [Test]
    public async Task AsyncActionCanBeConvertedToAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        await ExtendedApi.After(
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
            async () => await ExtendedApi.After(
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

        await ExtendedApi.After(
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
            async () => await ExtendedApi.After(
                "My fixture",
                asyncErrorFunc
            ),
            Throws.Exception
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.failed);
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
        Assert.That(this.Context.HasFixture, Is.False);
        Assert.That(this.Context.HasContainer);
        var fixtures = getFixtures(this.Context.CurrentContainer);
        Assert.That(fixtures, Has.Count.EqualTo(1));
        var fixture = fixtures.First();
        Assert.That(fixture.name, Is.EqualTo(name));
        Assert.That(fixture.status, Is.EqualTo(status));
    }
}
