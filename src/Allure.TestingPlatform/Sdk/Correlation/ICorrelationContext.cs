namespace Allure.TestingPlatform.Sdk.Correlation;

public interface ICorrelationContext
{
    CorrelationUid CurrentCorrelationUid { get; }
}
