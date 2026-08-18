namespace Allure.TestingPlatform.Sdk.ExecutionState;

/// <summary>
/// Identifies an Allure execution state of a test.
/// </summary>
/// <param name="Value">The execution state identifier value.</param>
public readonly record struct TestExecutionStateUid(string Value) : IAllureExecutionStateUid;
