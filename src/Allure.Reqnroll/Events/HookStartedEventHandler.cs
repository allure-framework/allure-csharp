using System;
using Allure.ReqnrollPlugin.State;
using Reqnroll;
using Reqnroll.Bindings;
using Reqnroll.Events;
using Reqnroll.Infrastructure;

namespace Allure.ReqnrollPlugin.Events;

internal class HookStartedEventHandler : AllureReqnrollEventHandler<HookStartedEvent>
{
    readonly Lazy<ITestRunnerManager> runnerManager;
    readonly Lazy<IContextManager> reqnrollContexts;

    ITestRunnerManager RunnerManager { get => this.runnerManager.Value; }

    IContextManager ReqnrollContexts { get => this.reqnrollContexts.Value; }

    public HookStartedEventHandler(
        Func<CrossBindingContextTransport> stateStorageFactory,
        Func<ITestRunnerManager> runnerManagerFactory,
        Func<IContextManager> reqnrollContextFactory
    ) : base(stateStorageFactory)
    {
        this.runnerManager = new(runnerManagerFactory);
        this.reqnrollContexts = new(reqnrollContextFactory);
    }

    protected override void HandleInAllureContext(HookStartedEvent eventData)
    {
        switch (eventData.HookType)
        {
            case HookType.BeforeFeature:
                AllureReqnrollStateFacade.StartContainer();
                break;
            case HookType.BeforeScenario:
                AllureReqnrollStateFacade.ScheduleScenario(
                    this.RunnerManager,
                    eventData.FeatureContext.FeatureInfo,
                    this.ReqnrollContexts.ScenarioContext
                );
                break;
            case HookType.AfterScenario:
                AllureReqnrollStateFacade.StopTestCase();
                break;
        }
    }
}
