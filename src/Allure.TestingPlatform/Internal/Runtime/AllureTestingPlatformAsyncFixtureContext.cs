using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Functions;
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

sealed class AllureTestingPlatformAsyncFixtureContext(
    IAllureTestingPlatformRegistration registration,
    FixtureExecutionStateUid fixtureUid
) :
    IAllureInProcessAsyncFixtureContext,
    IDataProducer,
    IDisposable
{
    int disposed = 0;

    readonly IAllureTestingPlatformMessageChannel channel = registration.MessageChannel;

    readonly IDisposable scope =
        registration.RuntimeReference.Value.ExecutionStateContext.EnterApiFixtureScope(fixtureUid);

    readonly ICorrelationContext correlationContext =
        registration.RuntimeReference.Value.CorrelationContext;

    public IAllureParameterSerializer ParameterSerializer =>
        registration.RuntimeReference.Value.ParameterSerializer;

    public Type[] DataTypesProduced => [typeof(AllureFixtureUpdateMessage)];

    public string Uid => "007eda41-53f5-40fe-a442-58eb265f696d";

    public string Version { get; } =
        PackageVersions.For(typeof(AllureTestingPlatformAsyncStepContext));

    public string DisplayName => "Allure fixture context";

    public string Description => "Allure fixture context that publishes step update messages to the MTP message bus.";

    public async Task AddParameterAsync(Parameter parameter, CancellationToken _)
    {
        this.ThrowIfDisposed();

        AllureFixtureUpdateMessage message = new(this.correlationContext.CurrentCorrelationUid, fixtureUid)
        {
            Properties = [new AllureParametersProperty<FixtureResult>([parameter])],
        };
        await this.channel.PublishAsync(this, message);
    }

    public async Task SetNameAsync(string newName, CancellationToken _)
    {
        this.ThrowIfDisposed();

        AllureFixtureUpdateMessage message = new(this.correlationContext.CurrentCorrelationUid, fixtureUid)
        {
            Properties = [new AllureNameProperty<FixtureResult>(newName)],
        };
        await this.channel.PublishAsync(this, message);
    }

    public bool TryReadFixtureResult<TResult>(Func<FixtureResult, TResult> read, out TResult value)
    {
        this.ThrowIfDisposed();

        throw new NotImplementedException(
            "In process model operations are not supported yet."
        );
    }

    public void UpdateFixtureResult(Action<FixtureResult> update)
    {
        this.ThrowIfDisposed();

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

    void ThrowIfDisposed()
    {
        if (Volatile.Read(ref this.disposed) != 0)
        {
            throw new ObjectDisposedException(this.GetType().FullName);
        }
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
}
