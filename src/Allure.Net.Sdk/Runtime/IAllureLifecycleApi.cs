using Allure.Model;

namespace Allure.Sdk.Runtime;

public interface IAllureLifecycleApi
{
    void StartScope(TestResultScope scope);

    TestResultScope StopScope();

    void StartBeforeFixture(FixtureResult fixtureResult);

    void StartAfterFixture(FixtureResult fixtureResult);

    FixtureResult StopFixture();

    void ScheduleTest(TestResult testResult);

    void StartTest();

    void StartTest(TestResult testResult);

    TestResult StopTest();

    void StartStep(StepResult stepResult);

    StepResult StopStep();
}
