using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Tests.OperationTests;

enum OperationTarget
{
    Test,
    Fixture,
    Step,
}

sealed class OperationExecutionContext :
    ExecutionStateContext,
    ICorrelationContext,
    ICorrelationStrategy
{
    readonly AsyncLocal<OperationTarget?> currentTarget = new();

    public CorrelationUid CurrentCorrelationUid { get; } = new("operations");

    public ScopeExecutionStateUid ScopeUid { get; } = new("scope");

    public TestExecutionStateUid TestUid { get; } = new("test");

    public FixtureExecutionStateUid FixtureUid { get; } = new("fixture");

    public StepExecutionStateUid StepUid { get; } = new("step");

    public bool IsActive => this.currentTarget.Value.HasValue;

    public override ScopeExecutionStateUid? CurrentScopeUid =>
        this.IsActive ? this.ScopeUid : null;

    public override TestExecutionStateUid? CurrentTestUid =>
        this.IsActive ? this.TestUid : null;

    protected override FixtureExecutionStateUid? CurrentFrameworkFixtureUid =>
        this.currentTarget.Value is OperationTarget.Fixture ? this.FixtureUid : null;

    protected override StepExecutionStateUid? CurrentFrameworkStepUid =>
        this.currentTarget.Value is OperationTarget.Step ? this.StepUid : null;

    public IDisposable Enter(OperationTarget target)
    {
        var previous = this.currentTarget.Value;
        this.currentTarget.Value = target;
        return new Scope(this.currentTarget, previous);
    }

    public Task<CorrelationUid?> GetCorrelationAsync(
        IDataProducer dataProducer,
        DataWithSessionUid message,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult<CorrelationUid?>(this.CurrentCorrelationUid);

    sealed class Scope(
        AsyncLocal<OperationTarget?> currentTarget,
        OperationTarget? previous
    ) : IDisposable
    {
        int disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.disposed, 1) == 0)
            {
                currentTarget.Value = previous;
            }
        }
    }
}
