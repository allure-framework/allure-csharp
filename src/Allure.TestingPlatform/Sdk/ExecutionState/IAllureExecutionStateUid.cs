namespace Allure.TestingPlatform.Sdk.ExecutionState;

/// <summary>
/// Identifies an Allure execution state.
/// </summary>
public interface IAllureExecutionStateUid
{
    /// <summary>
    /// Gets the execution state identifier value.
    /// </summary>
    string Value { get; }
}
