using System;
using Allure.ReqnrollPlugin.State;
using Reqnroll.Bindings;
using Reqnroll.Events;

namespace Allure.ReqnrollPlugin.Events;

internal class HookBindingFinishedEventHandler
    : AllureReqnrollEventHandler<HookBindingFinishedEvent>
{
    public HookBindingFinishedEventHandler(
        Func<CrossBindingContextTransport> stateStorageFactory
    ) : base(stateStorageFactory)
    {
    }

    protected override void HandleInAllureContext(
        HookBindingFinishedEvent eventData
    )
    {
        switch (eventData.HookBinding.HookType)
        {
            case HookType.BeforeFeature:
            case HookType.AfterFeature:
            case HookType.BeforeScenario:
            case HookType.AfterScenario:
                // We don't have enough information here to infer the fixture's
                // status. The snapshot allows us to revisit the fixture in the
                // follow-up HookFinishedEvent in case the fixture fails.
                this.StateStorage.MakeSnapshot(eventData.HookBinding.Method);
                AllureReqnrollStateFacade.StopFixture();
                break;
            case HookType.AfterStep:
                break;
        }
    }
}
