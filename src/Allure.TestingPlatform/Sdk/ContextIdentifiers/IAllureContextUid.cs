namespace Allure.TestingPlatform.Sdk.ContextIdentifiers;

/// <summary>
/// Identifies an Allure lifecycle context.
/// </summary>
public interface IAllureContextUid
{
    /// <summary>
    /// Gets the context identifier value.
    /// </summary>
    string Value { get; }
}
