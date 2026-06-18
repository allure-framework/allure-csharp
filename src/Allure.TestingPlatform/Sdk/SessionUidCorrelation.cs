using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk;

public class SessionUidCorrelation : ICorrelationDefinition
{
    public CorrelationUid? ForTestNodeUpdateMessage(
        TestNodeUpdateMessage testNodeUpdateMessage
    ) =>
        new(testNodeUpdateMessage.SessionUid.Value);
}
