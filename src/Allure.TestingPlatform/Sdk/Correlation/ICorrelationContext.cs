namespace Allure.TestingPlatform.Sdk.Correlation;

/// <summary>
/// Provides the correlation identifier associated with the current execution context.
/// </summary>
public interface ICorrelationContext
{
    /// <summary>
    /// Gets the current correlation identifier.
    /// </summary>
    CorrelationUid CurrentCorrelationUid { get; }
}
