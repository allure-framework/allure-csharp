using System;
using System.Collections.Generic;
using System.Text;
using Allure.ReqnrollPlugin.State;
using Reqnroll.Events;

namespace Allure.ReqnrollPlugin.Events;

internal class FeatureFinishedEventHandler : AllureReqnrollEventHandler<FeatureFinishedEvent>
{
    public FeatureFinishedEventHandler(
        Func<CrossBindingContextTransport> stateStorageFactory
    ) : base(stateStorageFactory)
    {
    }

    protected override void HandleInAllureContext(
        FeatureFinishedEvent eventData
    ) =>
        AllureReqnrollStateFacade.EmitFeatureFiles();
}
