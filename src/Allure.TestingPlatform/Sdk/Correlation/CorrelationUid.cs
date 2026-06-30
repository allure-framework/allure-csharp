namespace Allure.TestingPlatform.Sdk.Correlation;

/// <summary>
/// Identifies a correlated stream of Microsoft Testing Platform and Allure SDK messages.
/// </summary>
/// <param name="Value">The correlation value.</param>
public readonly record struct CorrelationUid(string Value);
