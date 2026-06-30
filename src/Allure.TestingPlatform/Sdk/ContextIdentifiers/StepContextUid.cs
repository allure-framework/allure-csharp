namespace Allure.TestingPlatform.Sdk.ContextIdentifiers;

/// <summary>
/// Identifies an Allure step context.
/// </summary>
/// <param name="Value">The context identifier value.</param>
public readonly record struct StepContextUid(string Value) : IAllureContextUid;
