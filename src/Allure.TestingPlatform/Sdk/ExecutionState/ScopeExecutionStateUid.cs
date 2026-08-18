namespace Allure.TestingPlatform.Sdk.ExecutionState;

/// <summary>
/// Identifies an Allure execution state of a scope.
/// </summary>
/// <param name="Value">The execution state identifier value.</param>
public readonly record struct ScopeExecutionStateUid(string Value) : IAllureExecutionStateUid;
