using System.Linq;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk;

public class TestMetadataCorrelation : ICorrelationDefinition
{
    public CorrelationUid? ForTestNodeUpdateMessage(
        TestNodeUpdateMessage testNodeUpdateMessage
    ) =>
        testNodeUpdateMessage
            .TestNode
            .Properties
            .OfType<TestMetadataProperty>()
            .FirstOrDefault(
                static (meta) => meta.Key == METADATA_KEY
            )
            ?.Value switch
        {
            null => null,
            var value => new(value),
        };

    const string METADATA_KEY = "Allure.TestingPlatform.CorrelationUid";
}
