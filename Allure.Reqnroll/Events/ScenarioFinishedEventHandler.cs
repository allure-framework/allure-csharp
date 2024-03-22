using System;
using Allure.ReqnrollPlugin.SelectiveRun;
using Allure.ReqnrollPlugin.State;
using Reqnroll;
using Reqnroll.Events;

namespace Allure.ReqnrollPlugin.Events;

internal class ScenarioFinishedEventHandle
    : AllureReqnrollEventHandler<ScenarioFinishedEvent>
{
    public ScenarioFinishedEventHandle(
        Func<CrossBindingContextTransport> stateStorageFactory
    ) : base(stateStorageFactory)
    {

    }

    protected override void HandleInAllureContext(
        ScenarioFinishedEvent eventData
    )
    {
        if (this.IsScenarioSelected(eventData))
        {
            AllureReqnrollStateFacade.EmitScenarioFiles(
                eventData.ScenarioContext
            );
        }
    }

    bool IsScenarioSelected(ScenarioFinishedEvent eventData) =>
        TestPlanAwareTestRunner.IsScenarioSelected(
            eventData.ScenarioContext as ScenarioContext
        );
}
