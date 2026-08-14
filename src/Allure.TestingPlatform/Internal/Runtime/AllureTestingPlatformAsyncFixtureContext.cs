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
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Internal.Runtime;

using IAllureTestingPlatformRuntimeHandle = IAllureTestingPlatformRuntimeHandle<
    AllureTestingPlatformConfiguration,
    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
>;

class AllureTestingPlatformAsyncFixtureContext(
    IAllureTestingPlatformRuntimeHandle runtimeHandle,
    FixtureExecutionStateUid fixtureUid
) :
    IAllureInProcessAsyncFixtureContext,
    IDataProducer,
    IDisposable
{
    int disposed = 0;

    readonly IDisposable scope =
        runtimeHandle.RuntimeReference.Value.ExecutionStateContext.EnterApiFixtureScope(fixtureUid);

    readonly ICorrelationContext correlationContext =
        runtimeHandle.RuntimeReference.Value.CorrelationContext;

    public IAllureParameterSerializer ParameterSerializer =>
        runtimeHandle.RuntimeReference.Value.ParameterSerializer;

    public Type[] DataTypesProduced => [typeof(AllureStepUpdateMessage)];

    public string Uid => "007eda41-53f5-40fe-a442-58eb265f696d";

    public string Version => "1.0.0";

    public string DisplayName => "Allure fixture context";

    public string Description => "Allure fixture context that publishes step update messages to the MTP message bus.";

    public async Task AddParameterAsync(Parameter parameter, CancellationToken _)
    {
        AllureFixtureUpdateMessage message = new(this.correlationContext.CurrentCorrelationUid, fixtureUid)
        {
            Properties = [new AllureParametersProperty<FixtureResult>([parameter])],
        };
        await runtimeHandle.PublishAsync(this, message);
    }

    public async Task SetNameAsync(string newName, CancellationToken _)
    {
        AllureFixtureUpdateMessage message = new(this.correlationContext.CurrentCorrelationUid, fixtureUid)
        {
            Properties = [new AllureNameProperty<FixtureResult>(newName)],
        };
        await runtimeHandle.PublishAsync(this, message);
    }

    public bool TryReadFixtureResult<TResult>(Func<FixtureResult, TResult> read, out TResult value)
    {
        throw new NotImplementedException(
            "In process model operations are not supported yet."
        );
    }

    public void UpdateFixtureResult(Action<FixtureResult> update)
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
