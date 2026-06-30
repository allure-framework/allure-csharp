namespace Allure.TestingPlatform.Sdk.ContextIdentifiers;

/// <summary>
/// Identifies an Allure fixture context.
/// </summary>
/// <param name="Value">The context identifier value.</param>
public readonly record struct FixtureContextUid(string Value) : IAllureContextUid;
