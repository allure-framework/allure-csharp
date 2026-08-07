using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Internal.Correlation;

class NullCorrelationContext : ICorrelationContext
{
    public CorrelationUid CurrentCorrelationUid =>
        throw new System.InvalidOperationException(
            "This integration does not support Allure API."
        );

    public static NullCorrelationContext Instance { get; } = new();
}
