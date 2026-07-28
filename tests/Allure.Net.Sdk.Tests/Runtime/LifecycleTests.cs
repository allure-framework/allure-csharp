using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.Runtime;

public class LifecycleTests
{
    [Test]
    public async Task ShouldRunNestedScopesAndAttachScheduledTestToAllScopes()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var outer = new TestResultScope { Uuid = "outer", Name = "outer" };
        var inner = new TestResultScope { Uuid = "inner", Name = "inner" };
        var test = new AllureTestResult { Uuid = "test", Name = "test" };

        runtime.LifecycleApi.StartScope(outer);
        runtime.LifecycleApi.StartScope(inner);
        runtime.LifecycleApi.ScheduleTest(test);

        await Assert.That(runtime.ContextApi.CurrentState.ScopeDepth).IsEqualTo(2);
        await Assert.That(runtime.ContextApi.CurrentState.HasTest).IsTrue();
        await Assert.That(test.Stage).IsEqualTo(Stage.Scheduled);
        await Assert.That(outer.Children).IsEquivalentTo(["test"]);
        await Assert.That(inner.Children).IsEquivalentTo(["test"]);

        await Assert.That(runtime.LifecycleApi.StopTest()).IsSameReferenceAs(test);
        await Assert.That(runtime.LifecycleApi.StopScope()).IsSameReferenceAs(inner);
        await Assert.That(runtime.LifecycleApi.StopScope()).IsSameReferenceAs(outer);
    }

    [Test]
    public async Task ShouldAttachBeforeAndAfterFixturesToCurrentScope()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var scope = new TestResultScope { Uuid = "scope", Name = "scope" };
        var before = new FixtureResult { Name = "before" };
        var after = new FixtureResult { Name = "after" };
        runtime.LifecycleApi.StartScope(scope);

        runtime.LifecycleApi.StartBeforeFixture(before);
        var stoppedBefore = runtime.LifecycleApi.StopFixture();
        runtime.LifecycleApi.StartAfterFixture(after);
        var stoppedAfter = runtime.LifecycleApi.StopFixture();

        await Assert.That(stoppedBefore).IsSameReferenceAs(before);
        await Assert.That(stoppedAfter).IsSameReferenceAs(after);
        await Assert.That(scope.Befores).IsEquivalentTo([before]);
        await Assert.That(scope.Afters).IsEquivalentTo([after]);
        await Assert.That(runtime.ContextApi.CurrentState.HasFixture).IsFalse();
    }

    [Test]
    public async Task ShouldRunTestAndNestedStepsWithStagesAndTimestamps()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var test = new AllureTestResult { Uuid = "test", Name = "test" };
        var outer = new StepResult { Name = "outer" };
        var inner = new StepResult { Name = "inner" };

        var beforeTestStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        runtime.LifecycleApi.StartTest(test);
        var beforeOuterStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        runtime.LifecycleApi.StartStep(outer);
        var beforeInnerStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        runtime.LifecycleApi.StartStep(inner);
        var afterInnerStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        await Assert.That(test.Stage).IsEqualTo(Stage.Running);
        await Assert.That(outer.Stage).IsEqualTo(Stage.Running);
        await Assert.That(inner.Stage).IsEqualTo(Stage.Running);
        await Assert.That(test.Start).IsBetween(beforeTestStart, beforeOuterStart);
        await Assert.That(outer.Start).IsBetween(beforeOuterStart, beforeInnerStart);
        await Assert.That(inner.Start).IsBetween(beforeInnerStart, afterInnerStart);
        await Assert.That(runtime.ContextApi.CurrentState.StepDepth).IsEqualTo(2);
        await Assert.That(test.Steps).IsEquivalentTo([outer]);
        await Assert.That(outer.Steps).IsEquivalentTo([inner]);

        var beforeInnerStop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await Assert.That(runtime.LifecycleApi.StopStep()).IsSameReferenceAs(inner);
        var beforeOuterStop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await Assert.That(runtime.LifecycleApi.StopStep()).IsSameReferenceAs(outer);
        var beforeTestStop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await Assert.That(runtime.LifecycleApi.StopTest()).IsSameReferenceAs(test);
        var afterTestStop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await Assert.That(test.Stage).IsEqualTo(Stage.Finished);
        await Assert.That(outer.Stage).IsEqualTo(Stage.Finished);
        await Assert.That(inner.Stage).IsEqualTo(Stage.Finished);
        await Assert.That(inner.Stop).IsBetween(beforeInnerStop, beforeOuterStop);
        await Assert.That(outer.Stop).IsBetween(beforeOuterStop, beforeTestStop);
        await Assert.That(test.Stop).IsBetween(beforeTestStop, afterTestStop);
        await Assert.That(runtime.ContextApi.CurrentState.HasTest).IsFalse();
        await Assert.That(runtime.ContextApi.CurrentState.HasStep).IsFalse();
    }

    [Test]
    public async Task ShouldPreserveCallerSuppliedTimestamps()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var test = new AllureTestResult
        {
            Uuid = "test",
            Name = "test",
            Start = 100,
            Stop = 200,
        };

        runtime.LifecycleApi.StartTest(test);
        runtime.LifecycleApi.StopTest();

        await Assert.That(test.Start).IsEqualTo(100);
        await Assert.That(test.Stop).IsEqualTo(200);
    }

    [Test]
    public async Task ShouldStartPreviouslyScheduledTest()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var test = new AllureTestResult { Uuid = "test", Name = "test" };
        runtime.LifecycleApi.ScheduleTest(test);

        runtime.LifecycleApi.StartTest();

        await Assert.That(test.Stage).IsEqualTo(Stage.Running);
        await Assert.That(test.Start).IsGreaterThan(0);
    }

    [Test]
    public async Task ShouldRejectFixtureWithoutScope()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;

        await Assert.That(
            () => runtime.LifecycleApi.StartBeforeFixture(new() { Name = "fixture" })
        ).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldRejectSecondActiveFixture()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        runtime.LifecycleApi.StartScope(new() { Uuid = "scope", Name = "scope" });
        runtime.LifecycleApi.StartBeforeFixture(new() { Name = "first" });

        await Assert.That(
            () => runtime.LifecycleApi.StartAfterFixture(new() { Name = "second" })
        ).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldRejectSecondActiveTest()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        runtime.LifecycleApi.StartTest(new() { Uuid = "first", Name = "first" });

        await Assert.That(
            () => runtime.LifecycleApi.StartTest(
                new() { Uuid = "second", Name = "second" }
            )
        ).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldRejectStepWithoutFixtureOrTest()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;

        await Assert.That(
            () => runtime.LifecycleApi.StartStep(new() { Name = "step" })
        ).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldRejectScopeChangesWhileExecutableItemIsActive()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        runtime.LifecycleApi.StartScope(new() { Uuid = "scope", Name = "scope" });
        runtime.LifecycleApi.StartTest(new() { Uuid = "test", Name = "test" });

        await Assert.That(
            () => runtime.LifecycleApi.StartScope(
                new() { Uuid = "nested", Name = "nested" }
            )
        ).Throws<InvalidOperationException>();
        await Assert.That(runtime.LifecycleApi.StopScope)
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task ShouldRejectStoppingAbsentLifecycleItems()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;

        await Assert.That(runtime.LifecycleApi.StopScope)
            .Throws<InvalidOperationException>();
        await Assert.That(runtime.LifecycleApi.StopFixture)
            .Throws<InvalidOperationException>();
        await Assert.That(runtime.LifecycleApi.StopTest)
            .Throws<InvalidOperationException>();
        await Assert.That(runtime.LifecycleApi.StopStep)
            .Throws<InvalidOperationException>();
    }
}
