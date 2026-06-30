namespace Allure.TestingPlatform.Sdk.ContextIdentifiers;

/// <summary>
/// Identifies an Allure test context.
/// </summary>
/// <param name="Value">The context identifier value.</param>
public readonly record struct TestContextUid(string Value) : IAllureContextUid;
