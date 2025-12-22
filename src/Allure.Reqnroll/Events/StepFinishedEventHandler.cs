using System;
using Allure.ReqnrollPlugin.State;
using Reqnroll.Events;

namespace Allure.ReqnrollPlugin.Events;
class StepFinishedEventHandler : AllureReqnrollEventHandler<StepFinishedEvent>
{
    public StepFinishedEventHandler(
        Func<CrossBindingContextTransport> stateStorageFactory
    ) : base(stateStorageFactory)
    {
    }

    protected override void HandleInAllureContext(
        StepFinishedEvent eventData
    ) =>
        AllureReqnrollStateFacade.StopStep(
            eventData.ScenarioContext,
            eventData.StepContext
        );
}
