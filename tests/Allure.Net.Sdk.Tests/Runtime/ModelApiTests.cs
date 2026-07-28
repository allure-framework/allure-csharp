using Allure.Model;
using Allure.Net.Sdk.Tests.Infrastructure;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.Runtime;

public class ModelApiTests
{
    [Test]
    public async Task ShouldReadAndUpdateScopesByCurrentLevelAndAll()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var outer = new TestResultScope { Uuid = "outer", Name = "outer" };
        var inner = new TestResultScope { Uuid = "inner", Name = "inner" };
        runtime.LifecycleApi.StartScope(outer);
        runtime.LifecycleApi.StartScope(inner);

        runtime.ModelApi.UpdateScope(scope => scope.Name += "-current");
        runtime.ModelApi.UpdateScope(0, scope => scope.Name += "-level-0");
        runtime.ModelApi.UpdateScope(1, scope => scope.Name += "-level-1");
        runtime.ModelApi.UpdateAllScopes(scope => scope.Name += "-all");

        await Assert.That(runtime.ModelApi.ReadScope(scope => scope.Uuid))
            .IsEqualTo("inner");
        await Assert.That(runtime.ModelApi.ReadScope(0, scope => scope.Uuid))
            .IsEqualTo("outer");
        await Assert.That(runtime.ModelApi.ReadScope(1, scope => scope.Uuid))
            .IsEqualTo("inner");
        await Assert.That(runtime.ModelApi.ReadAllScopes(scope => scope.Uuid))
            .IsEquivalentTo(["inner", "outer"]);
        await Assert.That(outer.Name).IsEqualTo("outer-level-0-all");
        await Assert.That(inner.Name)
            .IsEqualTo("inner-current-level-1-all");
    }

    [Test]
    public async Task ShouldReadAndUpdateStepsByCurrentLevelAndAll()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var test = new AllureTestResult { Uuid = "test", Name = "test" };
        var outer = new StepResult { Name = "outer" };
        var inner = new StepResult { Name = "inner" };
        runtime.LifecycleApi.StartTest(test);
        runtime.LifecycleApi.StartStep(outer);
        runtime.LifecycleApi.StartStep(inner);

        runtime.ModelApi.UpdateStepResult(step => step.Name += "-current");
        runtime.ModelApi.UpdateStepResult(0, step => step.Name += "-level-0");
        runtime.ModelApi.UpdateStepResult(1, step => step.Name += "-level-1");
        runtime.ModelApi.UpdateAllStepResults(step => step.Name += "-all");

        await Assert.That(runtime.ModelApi.ReadStepResult(step => step))
            .IsSameReferenceAs(inner);
        await Assert.That(runtime.ModelApi.ReadStepResult(0, step => step))
            .IsSameReferenceAs(outer);
        await Assert.That(runtime.ModelApi.ReadStepResult(1, step => step))
            .IsSameReferenceAs(inner);
        await Assert.That(runtime.ModelApi.ReadAllSteps(step => step))
            .IsEquivalentTo([inner, outer]);
        await Assert.That(outer.Name).IsEqualTo("outer-level-0-all");
        await Assert.That(inner.Name)
            .IsEqualTo("inner-current-level-1-all");
    }

    [Test]
    public async Task ShouldReadAndUpdateFixtureAndTest()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var scope = new TestResultScope { Uuid = "scope", Name = "scope" };
        var fixture = new FixtureResult { Name = "fixture" };
        runtime.LifecycleApi.StartScope(scope);
        runtime.LifecycleApi.StartBeforeFixture(fixture);
        runtime.ModelApi.UpdateFixtureResult(result => result.Name = "updated-fixture");

        await Assert.That(runtime.ModelApi.ReadFixtureResult(result => result))
            .IsSameReferenceAs(fixture);
        await Assert.That(fixture.Name).IsEqualTo("updated-fixture");

        runtime.LifecycleApi.StopFixture();
        var test = new AllureTestResult { Uuid = "test", Name = "test" };
        runtime.LifecycleApi.StartTest(test);
        runtime.ModelApi.UpdateTestResult(result => result.Name = "updated-test");

        await Assert.That(runtime.ModelApi.ReadTestResult(result => result))
            .IsSameReferenceAs(test);
        await Assert.That(test.Name).IsEqualTo("updated-test");
    }

    [Test]
    public async Task ShouldSelectStepFixtureThenTestAsCurrentExecutableItem()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;
        var scope = new TestResultScope { Uuid = "scope", Name = "scope" };
        var fixture = new FixtureResult { Name = "fixture" };
        runtime.LifecycleApi.StartScope(scope);
        runtime.LifecycleApi.StartBeforeFixture(fixture);

        runtime.ModelApi.UpdateCurrentExecutableItem(item => item.Name += "-current");
        await Assert.That(
            runtime.ModelApi.ReadCurrentExecutableItem(item => item)
        ).IsSameReferenceAs(fixture);

        var fixtureStep = new StepResult { Name = "fixture-step" };
        runtime.LifecycleApi.StartStep(fixtureStep);
        await Assert.That(
            runtime.ModelApi.ReadCurrentExecutableItem(item => item)
        ).IsSameReferenceAs(fixtureStep);
        runtime.LifecycleApi.StopStep();
        runtime.LifecycleApi.StopFixture();

        var test = new AllureTestResult { Uuid = "test", Name = "test" };
        runtime.LifecycleApi.StartTest(test);
        await Assert.That(
            runtime.ModelApi.ReadCurrentExecutableItem(item => item)
        ).IsSameReferenceAs(test);
    }

    [Test]
    public async Task ShouldRejectMissingObjectsAndInvalidLevels()
    {
        var runtime = RuntimeTestEnvironment.Create().Runtime;

        await Assert.That(() => runtime.ModelApi.ReadScope(scope => scope))
            .Throws<InvalidOperationException>();
        await Assert.That(() => runtime.ModelApi.ReadFixtureResult(result => result))
            .Throws<InvalidOperationException>();
        await Assert.That(() => runtime.ModelApi.ReadTestResult(result => result))
            .Throws<InvalidOperationException>();
        await Assert.That(() => runtime.ModelApi.ReadStepResult(result => result))
            .Throws<InvalidOperationException>();
        await Assert.That(
            () => runtime.ModelApi.ReadCurrentExecutableItem(item => item)
        ).Throws<InvalidOperationException>();

        runtime.LifecycleApi.StartScope(new() { Uuid = "scope", Name = "scope" });
        await Assert.That(() => runtime.ModelApi.ReadScope(-1, scope => scope))
            .Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => runtime.ModelApi.ReadScope(1, scope => scope))
            .Throws<ArgumentOutOfRangeException>();
    }
}
