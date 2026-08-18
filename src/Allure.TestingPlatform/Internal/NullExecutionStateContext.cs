using Allure.TestingPlatform.Sdk.ExecutionState;

namespace Allure.TestingPlatform.Internal;

sealed class NullExecutionStateContext : ExecutionStateContext
{
    public override ScopeExecutionStateUid? CurrentScopeUid => throw new System.NotImplementedException(
        "This integration does not support Allure API."
    );

    public override TestExecutionStateUid? CurrentTestUid => throw new System.NotImplementedException(
        "This integration does not support Allure API."
    );

    protected override FixtureExecutionStateUid? CurrentFrameworkFixtureUid => throw new System.NotImplementedException(
        "This integration does not support Allure API."
    );

    protected override StepExecutionStateUid? CurrentFrameworkStepUid => throw new System.NotImplementedException(
        "This integration does not support Allure API."
    );

    public static NullExecutionStateContext Instance { get; } = new();
}