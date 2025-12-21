using System;
using Allure.ReqnrollPlugin.State;
using Reqnroll.Bindings;
using Reqnroll.Events;

namespace Allure.ReqnrollPlugin.Events;

internal class HookBindingStartedEventHandler
    : AllureReqnrollEventHandler<HookBindingStartedEvent>
{
    public HookBindingStartedEventHandler(
        Func<CrossBindingContextTransport> stateStorageFactory
    ) : base(stateStorageFactory)
    {

    }

    protected override void HandleInAllureContext(HookBindingStartedEvent eventData)
    {
        switch (eventData.HookBinding.HookType)
        {
            case HookType.BeforeFeature:
            case HookType.BeforeScenario:
                AllureReqnrollStateFacade.StartBeforeFixture(eventData.HookBinding);
                break;
            case HookType.AfterFeature:
            case HookType.AfterScenario:
                AllureReqnrollStateFacade.StartAfterFixture(eventData.HookBinding);
                break;
        }
    }
}
