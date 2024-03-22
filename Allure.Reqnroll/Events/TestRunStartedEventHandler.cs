using System;
using Allure.ReqnrollPlugin.State;
using Reqnroll.Events;

namespace Allure.ReqnrollPlugin.Events;

internal class TestRunStartedEventHandler
    : AllureReqnrollEventHandler<TestRunStartedEvent>
{
    public TestRunStartedEventHandler(
        Func<CrossBindingContextTransport> stateStorageFactory
    ) : base(stateStorageFactory)
    {

    }

    protected override void HandleInAllureContext(
        TestRunStartedEvent eventData
    ) => AllureReqnrollStateFacade.Initialize();
}
