using Allure.TestingPlatform.Sdk.ExecutionState;

namespace Allure.TestingPlatform.Internal;

sealed class NullExecutionStateContext(string runtimeName) : ExecutionStateContext
{
    readonly string message = $"{runtimeName} does not support Allure API.";

    public override ScopeExecutionStateUid? CurrentScopeUid =>
        throw new System.NotImplementedException(message);

    public override TestExecutionStateUid? CurrentTestUid =>
        throw new System.NotImplementedException(message);

    protected override FixtureExecutionStateUid? CurrentFrameworkFixtureUid =>
        throw new System.NotImplementedException(message);

    protected override StepExecutionStateUid? CurrentFrameworkStepUid =>
        throw new System.NotImplementedException(message);
}