using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Internal.Runtime;

using IAllureTestingPlatformRegistration = IAllureTestingPlatformRegistration<
    AllureTestingPlatformConfiguration,
    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
>;

sealed class AllureTestingPlatformAsyncStepContext(
    IAllureTestingPlatformRegistration registration,
    StepExecutionStateUid stepUid
) :
    IAllureInProcessAsyncStepContext,
    IDataProducer,
    IDisposable
{
    int disposed = 0;

    readonly IAllureTestingPlatformMessageChannel channel = registration.MessageChannel;

    readonly ICorrelationContext correlationContext =
        registration.RuntimeReference.Value.CorrelationContext;

    readonly IDisposable scope =
        registration.RuntimeReference.Value.ExecutionStateContext.EnterApiStepScope(stepUid);

    public IAllureParameterSerializer ParameterSerializer =>
        registration.RuntimeReference.Value.ParameterSerializer;

    public Type[] DataTypesProduced => [typeof(AllureStepUpdateMessage)];

    public string Uid => "90b49fbd-4c57-48c9-b196-849881e43199";

    public string Version => "1.0.0";

    public string DisplayName => "Allure step context";

    public string Description => "Allure step context that publishes step update messages to the MTP message bus.";

    public async Task AddParameterAsync(Parameter parameter, CancellationToken _)
    {
        AllureStepUpdateMessage message = new(this.correlationContext.CurrentCorrelationUid, stepUid)
        {
            Properties = [new AllureParametersProperty<StepResult>([parameter])],
        };
        await this.channel.PublishAsync(this, message);
    }

    public async Task SetNameAsync(string newName, CancellationToken _)
    {
        AllureStepUpdateMessage message = new(this.correlationContext.CurrentCorrelationUid, stepUid)
        {
            Properties = [new AllureNameProperty<StepResult>(newName)],
        };
        await this.channel.PublishAsync(this, message);
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

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
}
