using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Internal.Runtime;

class AllureTestingPlatformAsyncStepContext(
    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> runtime,
    StepExecutionStateUid stepUid
) :
    IAllureInProcessAsyncStepContext,
    IDataProducer,
    IDisposable
{
    int disposed = 0;

    readonly IDisposable scope = runtime.ExecutionStateContext.EnterApiStepScope(stepUid);

    public IAllureParameterSerializer ParameterSerializer => runtime.ParameterSerializer;

    public Type[] DataTypesProduced => [typeof(AllureStepUpdateMessage)];

    public string Uid => "90b49fbd-4c57-48c9-b196-849881e43199";

    public string Version => "1.0.0";

    public string DisplayName => "Allure step context";

    public string Description => "Allure step context that publishes step update messages to the MTP message bus.";

    public async Task AddParameterAsync(Parameter parameter, CancellationToken _)
    {
        AllureStepUpdateMessage message = new(runtime.CorrelationContext.CurrentCorrelationUid, stepUid)
        {
            Properties = [new AllureParametersProperty<StepResult>([parameter])],
        };
        await runtime.MessageBus.PublishAsync(this, message);
    }

    public async Task SetNameAsync(string newName, CancellationToken _)
    {
        AllureStepUpdateMessage message = new(runtime.CorrelationContext.CurrentCorrelationUid, stepUid)
        {
            Properties = [new AllureNameProperty<StepResult>(newName)],
        };
        await runtime.MessageBus.PublishAsync(this, message);
    }

    public bool TryReadStepResult<TResult>(Func<StepResult, TResult> read, out TResult value)
    {
        throw new NotImplementedException(
            "In process model operations are not supported yet."
        );
    }

    public void UpdateStepResult(Action<StepResult> update)
    {
        throw new NotImplementedException(
            "In process model operations are not supported yet."
        );
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref this.disposed, 1) != 0)
        {
            return;
        }

        this.scope.Dispose();
    }

    protected void EnsureInScope()
    {
        if (this.disposed != 0)
        {
            throw new InvalidOperationException(
                "The fixture associated with this context has already finished."
            );
        }
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
}
