namespace Allure.TestingPlatform.Sdk.ContextIdentifiers;

/// <summary>
/// Identifies an Allure scope context.
/// </summary>
/// <param name="Value">The context identifier value.</param>
public readonly record struct ScopeContextUid(string Value) : IAllureContextUid;
