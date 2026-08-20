using Allure.TestingPlatform.Sdk.ExecutionState;

namespace Allure.TestingPlatform.Internal;

sealed class NullExecutionStateContext : ExecutionStateContext
{
    public override ScopeExecutionStateUid? CurrentScopeUid => null;

    public override TestExecutionStateUid? CurrentTestUid => null;

    protected override FixtureExecutionStateUid? CurrentFrameworkFixtureUid => null;

    protected override StepExecutionStateUid? CurrentFrameworkStepUid => null;

    public static NullExecutionStateContext Instance { get; } = new();
}
