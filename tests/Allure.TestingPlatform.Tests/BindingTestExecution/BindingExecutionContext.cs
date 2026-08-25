using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Tests.BindingTestExecution;

sealed class BindingExecutionContext :
    ExecutionStateContext,
    ICorrelationContext,
    ICorrelationStrategy
{
    readonly AsyncLocal<ExecutionFrame> currentExecution = new();

    public BindingExecutionContext()
    {
        this.CurrentCorrelationUid = new($"binding-execution-{Guid.NewGuid():N}");
    }

    public CorrelationUid CurrentCorrelationUid { get; }

    public bool IsActive => this.currentExecution.Value is not null;

    public override ScopeExecutionStateUid? CurrentScopeUid =>
        this.currentExecution.Value?.ScopeUid;

    public override TestExecutionStateUid? CurrentTestUid =>
        this.currentExecution.Value?.ExecutionUid;

    protected override FixtureExecutionStateUid? CurrentFrameworkFixtureUid => null;

    protected override StepExecutionStateUid? CurrentFrameworkStepUid => null;

    public IDisposable Enter(
        TestExecutionStateUid executionUid,
        ScopeExecutionStateUid scopeUid
    )
    {
        var previous = this.currentExecution.Value;
        this.currentExecution.Value = new(executionUid, scopeUid);
        return new Scope(this.currentExecution, previous);
    }

    public Task<CorrelationUid?> GetCorrelationAsync(
        IDataProducer dataProducer,
        DataWithSessionUid message,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult<CorrelationUid?>(this.CurrentCorrelationUid);

    sealed record ExecutionFrame(
        TestExecutionStateUid ExecutionUid,
        ScopeExecutionStateUid ScopeUid
    );

    sealed class Scope(
        AsyncLocal<ExecutionFrame> currentExecution,
        ExecutionFrame previous
    ) : IDisposable
    {
        int disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.disposed, 1) == 0)
            {
                currentExecution.Value = previous;
            }
        }
    }
}
