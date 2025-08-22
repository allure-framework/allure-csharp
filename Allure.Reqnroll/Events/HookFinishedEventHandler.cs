using System;
using Allure.ReqnrollPlugin.Functions;
using Allure.ReqnrollPlugin.State;
using Reqnroll;
using Reqnroll.Bindings;
using Reqnroll.Events;

namespace Allure.ReqnrollPlugin.Events;

internal class HookFinishedEventHandler : AllureReqnrollEventHandler<HookFinishedEvent>
{
    readonly Lazy<ITestRunnerManager> runnerManager;

    ITestRunnerManager RunnerManager { get => this.runnerManager.Value; }

    public HookFinishedEventHandler(
        Func<CrossBindingContextTransport> stateStorageFactory,
        Func<ITestRunnerManager> runnerManagerFactory
    ) : base(stateStorageFactory)
    {
        this.runnerManager = new(runnerManagerFactory);
    }

    protected override void HandleInAllureContext(HookFinishedEvent eventData)
    {
        this.RevisitLastFixture(eventData);
        this.HandleEvent(eventData);
    }

    void RevisitLastFixture(HookFinishedEvent eventData)
    {
        if (IsFixtureHook(eventData))
        {
            // If the feature/scenario hook fails
            this.EnsureFixtureErrorReported(eventData);

            // We need to clear the snapshot to prevent interferance between
            // an error in an After hook and the snapshot from the last Before
            // hook.
            this.ClearFixtureSnapshot();
        }
    }

    void HandleEvent(HookFinishedEvent eventData)
    {
        switch (eventData.HookType)
        {
            case HookType.BeforeFeature:
                this.EnsureFeatureErrorReported(eventData);
                break;
            case HookType.BeforeScenario:
                AllureReqnrollStateFacade.StartTestCase();
                break;
            case HookType.AfterScenario:
            case HookType.AfterFeature:
                AllureReqnrollStateFacade.StopContainer();
                break;
        }
    }

    static bool IsFixtureHook(HookFinishedEvent eventData) =>
        eventData.HookType switch
        {
            HookType.BeforeFeature
                or HookType.BeforeScenario
                or HookType.AfterScenario
                or HookType.AfterFeature => true,
            _ => false
        };

    void EnsureFixtureErrorReported(HookFinishedEvent eventData)
    {
        var error = eventData.HookException;
        if (error is not null)
        {
            this.ReportHookFailure(eventData);
            this.EnsureScenarioReported(eventData);
        }
    }

    void ClearFixtureSnapshot() =>
        this.StateStorage.ClearSnapshot();

    void EnsureFeatureErrorReported(HookFinishedEvent eventData)
    {
        if (eventData.HookException is not null)
        {
            // Reqnroll silently skips all scenarios of a feature in case
            // BeforeFeature fails. There is no (reasonable) way to enumerate
            // scenarios manually and include them as skipped.
            // Instead, a placeholder scenario is created to report the error.
            AllureReqnrollStateFacade.ReportFeatureHookError(
                this.RunnerManager,
                eventData.HookType,
                eventData.FeatureContext.FeatureInfo,
                eventData.HookException
            );
        }
    }

    void ReportHookFailure(HookFinishedEvent eventData)
    {
        // The snapshot contains AllureContext for the last fixture and the
        // method that implements the corresponding binding.
        var snapshot = this.StateStorage.GetLastSnapshot();
        var error = eventData.HookException;
        if (IsThrownFromHook(snapshot, error))
        {
            // If the exception is thrown by the user's code, the snapshot
            // belongs to the relevant hook, because Reqnroll calls
            // HookBindingFinishedEvent in that case.
            AllureReqnrollStateFacade.FixSnapshottedFixtureStatus(
                snapshot!.State,
                error
            );
        }
        else
        {
            // If the exception is thrown by Reqnroll (e.g., because of invalid
            // signature), the snapshot either doesn't exist or belongs to a
            // previous (succeeded) hook. That's because Reqnroll doesn't call
            // HookBindingFinishedEvent in such cases. There is not enough
            // information about the failed hook. The best we can do is to
            // create a placeholder fixture to report the error.
            AllureReqnrollStateFacade.CreateFailedFixturePlaceHolder(
                eventData.HookType,
                error
            );
        }
    }

    void EnsureScenarioReported(HookFinishedEvent eventData)
    {
        if (eventData.HookType is HookType.AfterScenario)
        {
            // Reqnroll doesn't fire ScenarioFinishedEvent if an AfterScenario
            // hook fails. We need to emit scenario files here instead.
            AllureReqnrollStateFacade.EmitScenarioFiles(
                eventData.ScenarioContext
            );
        }
    }

    static bool IsThrownFromHook(StateSnapshot? snapshot, Exception error) =>
        snapshot is not null
            && ExceptionFunctions.IsFromHookMethod(error, snapshot.Origin);
}
