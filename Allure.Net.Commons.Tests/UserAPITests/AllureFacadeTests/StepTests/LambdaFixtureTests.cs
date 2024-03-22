using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

#nullable enable

namespace Allure.Net.Commons.Tests.UserAPITests.AllureFacadeTests.StepTests;

class LambdaFixtureTests : LambdaApiTestFixture
{
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
            () => ExtendedApi.Before("My fixture", failAction),
            Throws.InstanceOf<FailException>()
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.failed, "message", typeof(FailException));
    }

    [Test]
    public void ActionCanBeConvertedToBrokenBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => ExtendedApi.Before("My fixture", breakAction),
            Throws.Exception
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.broken, "message", typeof(Exception));
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
            () => ExtendedApi.Before("My fixture", failFunc),
            Throws.InstanceOf<FailException>()
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.failed, "message", typeof(FailException));
    }

    [Test]
    public void FuncCanBeConvertedToBrokenBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => ExtendedApi.Before("My fixture", breakFunc),
            Throws.Exception
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.broken, "message", typeof(Exception));
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
                asyncFailAction
            ),
            Throws.InstanceOf<FailException>()
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.failed, "message", typeof(FailException));
    }

    [Test]
    public void AsyncActionCanBeConvertedToBrokenBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            async () => await ExtendedApi.Before(
                "My fixture",
                asyncBreakAction
            ),
            Throws.Exception
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.broken, "message", typeof(Exception));
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
                asyncFailFunc
            ),
            Throws.InstanceOf<FailException>()
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.failed, "message", typeof(FailException));
    }

    [Test]
    public void AsyncFuncCanBeConvertedToBrokenBeforeFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            async () => await ExtendedApi.Before(
                "My fixture",
                asyncBreakFunc
            ),
            Throws.Exception
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.broken, "message", typeof(Exception));
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
            () => ExtendedApi.After("My fixture", failAction),
            Throws.InstanceOf<FailException>()
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.failed, "message", typeof(FailException));
    }

    [Test]
    public void ActionCanBeConvertedToBrokenAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => ExtendedApi.After("My fixture", breakAction),
            Throws.Exception
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.broken, "message", typeof(Exception));
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
            () => ExtendedApi.After("My fixture", failFunc),
            Throws.InstanceOf<FailException>()
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.failed, "message", typeof(FailException));
    }

    [Test]
    public void FuncCanBeConvertedToBrokenAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => ExtendedApi.After("My fixture", breakFunc),
            Throws.Exception
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.broken, "message", typeof(Exception));
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
                asyncFailAction
            ),
            Throws.InstanceOf<FailException>()
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.failed, "message", typeof(FailException));
    }

    [Test]
    public void AsyncActionCanBeConvertedToBrokenAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            async () => await ExtendedApi.After(
                "My fixture",
                asyncBreakAction
            ),
            Throws.Exception
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.broken, "message", typeof(Exception));
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
                asyncFailFunc
            ),
            Throws.InstanceOf<FailException>()
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.failed, "message", typeof(FailException));
    }

    [Test]
    public void AsyncFuncCanBeConvertedToBrokenAfterFixture()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            async () => await ExtendedApi.After(
                "My fixture",
                asyncBreakFunc
            ),
            Throws.Exception
        );

        this.AssertAfterFixtureCompleted("My fixture", Status.broken, "message", typeof(Exception));
    }

    [Test]
    public void SubclassOfFailExceptionLeadsToFailed()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => ExtendedApi.Before("My fixture", () => throw new InheritedFailException()),
            Throws.InstanceOf<InheritedFailException>()
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.failed, "message", typeof(InheritedFailException));
    }

    [Test]
    public void ImplementationOfFailInterfaceTreatedAsAssertionFailure()
    {
        this.lifecycle.AllureConfiguration.FailExceptions = new()
        {
            typeof(IErrorInterface).FullName
        };
        this.lifecycle.StartTestContainer(new() { uuid = "uuid" });

        Assert.That(
            () => ExtendedApi.Before("My fixture", () => throw new InheritedFailException()),
            Throws.InstanceOf<InheritedFailException>()
        );

        this.AssertBeforeFixtureCompleted("My fixture", Status.failed, "message", typeof(InheritedFailException));
    }

    void AssertBeforeFixtureCompleted(
        string name,
        Status status,
        string? message = null,
        Type? exceptionType = null
    ) =>
        this.AssertFixtureCompleted(tc => tc.befores, name, status, message, exceptionType);

    void AssertAfterFixtureCompleted(
        string name,
        Status status,
        string? message = null,
        Type? exceptionType = null
    ) =>
        this.AssertFixtureCompleted(tc => tc.afters, name, status, message, exceptionType);

    void AssertFixtureCompleted(
        Func<TestResultContainer, List<FixtureResult>> getFixtures,
        string name,
        Status status,
        string? message = null,
        Type? exceptionType = null
    )
    {
        Assert.That(this.Context.HasFixture, Is.False);
        Assert.That(this.Context.HasContainer);
        var fixtures = getFixtures(this.Context.CurrentContainer);
        Assert.That(fixtures, Has.Count.EqualTo(1));
        var fixture = fixtures.First();
        Assert.That(fixture.name, Is.EqualTo(name));
        Assert.That(fixture.status, Is.EqualTo(status));
        Assert.That(fixture.statusDetails?.message, Is.EqualTo(message));
        if (message is not null)
        {
            Assert.That(fixture.statusDetails?.trace, Contains.Substring(message));
        }
        if (exceptionType?.FullName is not null)
        {
            Assert.That(fixture.statusDetails?.trace, Contains.Substring(exceptionType.FullName));
        }
    }
}
