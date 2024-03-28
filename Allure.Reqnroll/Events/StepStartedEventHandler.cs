using System;
using Allure.ReqnrollPlugin.State;
using Reqnroll.Events;

namespace Allure.ReqnrollPlugin.Events;

class StepStartedEventHandler : AllureReqnrollEventHandler<StepStartedEvent>
{
    public StepStartedEventHandler(
        Func<CrossBindingContextTransport> stateStorageFactory
    ) : base(stateStorageFactory)
    {
    }

    protected override void HandleInAllureContext(StepStartedEvent eventData) =>
        AllureReqnrollStateFacade.StartStep(
            eventData.StepContext.StepInfo
        );
}
