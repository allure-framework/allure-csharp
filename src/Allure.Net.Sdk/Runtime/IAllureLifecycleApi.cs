using Allure.Model;

namespace Allure.Sdk.Runtime;

/// <summary>
/// Controls the lifecycle of Allure result scopes, fixtures, tests, and steps.
/// </summary>
public interface IAllureLifecycleApi
{
    /// <summary>
    /// Starts a test result scope.
    /// </summary>
    void StartTestScope(TestResultScope scope);

    /// <summary>
    /// Stops and returns the current test result scope.
    /// </summary>
    TestResultScope StopTestScope();

    /// <summary>
    /// Starts a setup fixture in the current scope.
    /// </summary>
    void StartSetUpFixture(FixtureResult fixtureResult);

    /// <summary>
    /// Starts an teardown fixture in the current scope.
    /// </summary>
    void StartTearDownFixture(FixtureResult fixtureResult);

    /// <summary>
    /// Stops and returns the current fixture.
    /// </summary>
    FixtureResult StopFixture();

    /// <summary>
    /// Schedules a test in the current scope without starting it.
    /// </summary>
    void ScheduleTest(TestResult testResult);

    /// <summary>
    /// Starts the test previously scheduled in the current scope.
    /// </summary>
    void StartTest();

    /// <summary>
    /// Schedules and starts a test in the current scope.
    /// </summary>
    void StartTest(TestResult testResult);

    /// <summary>
    /// Stops and returns the current test.
    /// </summary>
    TestResult StopTest();

    /// <summary>
    /// Starts a step under the current fixture, test, or step.
    /// </summary>
    void StartStep(StepResult stepResult);

    /// <summary>
    /// Stops and returns the current step.
    /// </summary>
    StepResult StopStep();
}
