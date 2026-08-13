using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Internal.Runtime;

internal sealed class AllureTestingPlatformRuntimeCoordinator<
    TConfiguration,
    TRuntime,
    TIntegrationContext
>(
    string runtimeName,
    Func<
        AllureRuntimeRegistrationSessionBase<
            TConfiguration,
            TRuntime,
            TIntegrationContext
        >
    > sessionFactory,
    Action<TIntegrationContext, IServiceProvider> registration
) :
    IAllureTestingPlatformRuntimeHandle,
    IDisposable,
    IAsyncDisposable

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
    where TIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContextBase<
        TConfiguration,
        TRuntime
    >
{
    readonly object gate = new();

    readonly AllureRuntimeBuilder<
        TConfiguration,
        TRuntime,
        TIntegrationContext
    > runtimeBuilder = AllureRuntimeBuilder.Create(runtimeName, sessionFactory);

    readonly Action<TIntegrationContext, IServiceProvider> registration = registration
            ?? throw new ArgumentNullException(nameof(registration));

    readonly ServiceProviderProxy serviceProvider = new();

    IAllureRuntimeRegistrationPlan<TConfiguration, TRuntime>? registrationPlan;

    IAllureRuntimeRegistration<TRuntime>? runtimeRegistration;

    CoordinatorState state = CoordinatorState.Created;

    Exception? failure;

    public IReadOnlyLateBoundReference<TRuntime> RuntimeReference =>
        this.runtimeBuilder.RuntimeReference;

    public TConfiguration Configuration
    {
        get
        {
            lock (this.gate)
            {
                this.EnsurePrepared();
                return this.registrationPlan!.Configuration;
            }
        }
    }

    AllureTestingPlatformConfiguration IAllureTestingPlatformRuntimeHandle.Configuration => Configuration;

    IReadOnlyLateBoundReference<
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    > IAllureTestingPlatformRuntimeHandle.RuntimeReference =>
        (IReadOnlyLateBoundReference<
            IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
        >)this.RuntimeReference;

    public void Prepare(
        IServiceProvider initialServiceProvider
    )
    {
        if (initialServiceProvider is null)
        {
            throw new ArgumentNullException(nameof(initialServiceProvider));
        }

        lock (this.gate)
        {
            this.ThrowIfUnavailable();

            if (this.registrationPlan is not null)
            {
                return;
            }

            this.serviceProvider.BindInitial(initialServiceProvider);

            try
            {
                this.registrationPlan = this.runtimeBuilder.Prepare(
                    context => this.registration(context, this.serviceProvider)
                );
                this.state = CoordinatorState.Prepared;
            }
            catch (Exception exception)
            {
                this.failure = exception;
                this.state = CoordinatorState.Failed;
                throw;
            }
        }
    }

    public void BindTestHost(IServiceProvider testHostServiceProvider)
    {
        if (testHostServiceProvider is null)
        {
            throw new ArgumentNullException(nameof(testHostServiceProvider));
        }

        lock (this.gate)
        {
            this.EnsurePrepared();

            if (this.state is CoordinatorState.Built)
            {
                throw new InvalidOperationException(
                    "The Allure.TestingPlatform runtime has already been constructed."
                );
            }

            if (this.state is CoordinatorState.TestHostBound)
            {
                if (this.serviceProvider.IsBoundTo(testHostServiceProvider))
                {
                    return;
                }

                throw new InvalidOperationException(
                    "The Allure.TestingPlatform test-host service provider is already bound."
                );
            }

            this.serviceProvider.Rebind(testHostServiceProvider);
            this.state = CoordinatorState.TestHostBound;
        }
    }

    public void EnsureRuntimeStarted()
    {
        lock (this.gate)
        {
            this.EnsurePrepared();

            if (this.runtimeRegistration is not null)
            {
                return;
            }

            try
            {
                this.runtimeRegistration = this.registrationPlan!.Build();
                this.state = CoordinatorState.Built;
                return;
            }
            catch (Exception exception)
            {
                this.failure = exception;
                this.state = CoordinatorState.Failed;
                throw;
            }
        }
    }

    public void Dispose()
    {
        IAllureRuntimeRegistration<TRuntime>? registration;

        lock (this.gate)
        {
            if (this.state is CoordinatorState.Disposed)
            {
                return;
            }

            registration = this.runtimeRegistration;
            this.runtimeRegistration = null;
            this.state = CoordinatorState.Disposed;
        }

        registration?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        IAllureRuntimeRegistration<TRuntime>? registration;

        lock (this.gate)
        {
            if (this.state is CoordinatorState.Disposed)
            {
                return;
            }

            registration = this.runtimeRegistration;
            this.runtimeRegistration = null;
            this.state = CoordinatorState.Disposed;
        }

        if (registration is not null)
        {
            await registration.DisposeAsync().ConfigureAwait(false);
        }
    }

    void EnsurePrepared()
    {
        this.ThrowIfUnavailable();

        if (this.registrationPlan is null)
        {
            throw new InvalidOperationException(
                "The Allure.TestingPlatform runtime registration has not been prepared."
            );
        }
    }

    void ThrowIfUnavailable()
    {
        if (this.state is CoordinatorState.Disposed)
        {
            throw new ObjectDisposedException(this.GetType().FullName);
        }

        if (this.state is CoordinatorState.Failed)
        {
            throw new InvalidOperationException(
                "The Allure.TestingPlatform runtime registration has failed.",
                this.failure
            );
        }
    }

    enum CoordinatorState
    {
        Created,

        Prepared,

        TestHostBound,

        Built,

        Failed,

        Disposed,
    }

    sealed class ServiceProviderProxy : IServiceProvider
    {
        IServiceProvider? target;

        public object? GetService(Type serviceType)
        {
            var provider = Volatile.Read(ref this.target)
                ?? throw new InvalidOperationException(
                    "The service provider proxy has not been bound to its target."
                );

            return provider.GetService(serviceType);
        }

        internal void BindInitial(IServiceProvider provider)
        {
            if (this.target is not null)
            {
                throw new InvalidOperationException(
                    "The initial target of the service provider proxy is already established."
                );
            }

            Volatile.Write(ref this.target, provider);
        }

        internal void Rebind(IServiceProvider provider) =>
            Volatile.Write(ref this.target, provider);

        internal bool IsBoundTo(IServiceProvider provider) =>
            ReferenceEquals(Volatile.Read(ref this.target), provider);
    }
}

internal static class AllureTestingPlatformRuntimeCoordinator
{
    public static AllureTestingPlatformRuntimeCoordinator<
        TConfiguration,
        TRuntime,
        TIntegrationContext
    > Create<TConfiguration, TRuntime, TIntegrationContext>(
        string runtimeName,
        Func<
            AllureRuntimeRegistrationSessionBase<
                TConfiguration,
                TRuntime,
                TIntegrationContext
            >
        > sessionFactory,
        Action<TIntegrationContext, IServiceProvider> registration
    )
        where TConfiguration : AllureTestingPlatformConfiguration, new()
        where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
        where TIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContextBase<
            TConfiguration,
            TRuntime
        >
    =>
        new(runtimeName, sessionFactory, registration);
}
