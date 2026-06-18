using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk;

public interface ICorrelationDefinition
{
    CorrelationUid? ForTestNodeUpdateMessage(TestNodeUpdateMessage testNodeUpdateMessage);
}
