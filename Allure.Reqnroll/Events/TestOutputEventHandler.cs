using System;
using Allure.Net.Commons;
using Allure.ReqnrollPlugin.State;
using Reqnroll.Events;

namespace Allure.ReqnrollPlugin.Events;

class TestOutputEventHandler : AllureReqnrollEventHandler<OutputAddedEvent>
{
    public TestOutputEventHandler(
        Func<CrossBindingContextTransport> stateStorageFactory
    ) : base(stateStorageFactory)
    {
    }

    protected override void HandleInAllureContext(OutputAddedEvent eventData) {
        AllureReqnrollStateFacade.AddOutput(
            eventData.Text + "\n"
        );
    }
}