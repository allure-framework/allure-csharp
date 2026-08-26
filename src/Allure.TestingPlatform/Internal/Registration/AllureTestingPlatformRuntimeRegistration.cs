using System;
using System.Threading.Tasks;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Services;

namespace Allure.TestingPlatform.Internal.Registration;

sealed class AllureTestingPlatformRuntimeRegistration<
    TConfiguration,
    TRuntime,
    TIntegrationContext
> :
    IAllureTestingPlatformRegistrationControl<TConfiguration, TRuntime>,
    IAllureTestingPlatformRequestCoordinator,
    IDisposable,
    IAsyncDisposable

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
    where TIntegrationContext : IAllureTestingPlatformIntegrationContextBase<
        TConfiguration,
        TRuntime
    >
{
    readonly object gate = new();

    readonly AllureRuntimeBuilder<
        TConfiguration,
        TRuntime,
        TIntegrationContext
    > runtimeBuilder;

    readonly Action<
        TIntegrationContext,
        AllureTestingPlatformRuntimeRegistration<
            TConfiguration,
            TRuntime,
            TIntegrationContext
        >
    > registration;

    readonly ServiceProviderProxy serviceProvider;

    readonly MessageBusProxy messageChannel;

    readonly InternalServiceProvider<TConfiguration> internalServiceProvider;

    IAllureRuntimeRegistrationPlan<TConfiguration, TRuntime>? registrationPlan = null;

    IAllureRuntimeRegistration<TRuntime>? runtimeRegistration = null;

    RuntimeState runtimeState = RuntimeState.Created;

    BoundRole role = BoundRole.None;

    RequestBindingState requestState = RequestBindingState.Unbound;

    Exception? failure = null;

    ITestingPlatformRequestBinding? currentRequest = null;

    public IReadOnlyLateBoundReference<TConfiguration> ConfigurationReference =>
        this.runtimeBuilder.ConfigurationReference;

    public IReadOnlyLateBoundReference<TRuntime> RuntimeReference =>
        this.runtimeBuilder.RuntimeReference;

    public IAllureTestingPlatformMessageChannel MessageChannel => this.messageChannel;

    public IServiceProvider ServiceProvider => this.serviceProvider;

    public TConfiguration Configuration
    {
        get
        {
            lock (this.gate)
            {
                this.ThrowIfUnprepared();
                return this.registrationPlan!.Configuration;
            }
        }
    }

    public AllureTestingPlatformRuntimeRegistration(
        string runtimeName,
        Func<
            AllureRuntimeRegistrationSessionBase<
                TConfiguration,
                TRuntime,
                TIntegrationContext
            >
        > sessionFactory,
        Action<
            TIntegrationContext,
            AllureTestingPlatformRuntimeRegistration<
                TConfiguration,
                TRuntime,
                TIntegrationContext
            >
        > registration,
        InternalServiceProvider<TConfiguration> allureServiceProvider
    )
    {
        this.runtimeBuilder = AllureRuntimeBuilder.Create(runtimeName, sessionFactory);
        this.registration = registration
            ?? throw new ArgumentNullException(nameof(registration));
        this.messageChannel = new();
        this.serviceProvider = new(this.messageChannel);
        this.internalServiceProvider = allureServiceProvider;
    }

    public void EnsureRuntimeStarted()
    {
        lock (this.gate)
        {
            this.ThrowIfUnprepared();

            if (this.runtimeRegistration is not null)
            {
                return;
            }

            try
            {
                this.runtimeRegistration = this.registrationPlan!.Build();
                this.runtimeState = RuntimeState.Built;
                return;
            }
            catch (Exception exception)
            {
                this.failure = exception;
                this.runtimeState = RuntimeState.Failed;
                throw;
            }
        }
    }

    public void BindController(IServiceProvider serviceProvider)
    {
        if (serviceProvider is null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        lock (this.gate)
        {
            this.ThrowIfUnavailable();

            if (this.role is not BoundRole.None)
            {
                if (this.role is BoundRole.Controller && !this.serviceProvider.IsBoundTo(serviceProvider))
                {
                    throw new InvalidOperationException(
                        "The Allure.TestingPlatform runtime has already been bound to another controller."
                    );
                }

                return;
            }

            this.PrepareCore(serviceProvider, BoundRole.Controller);
        }
    }

    public void BindTestHost(IServiceProvider serviceProvider)
    {
        if (serviceProvider is null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        lock (this.gate)
        {
            this.ThrowIfUnavailable();

            if (this.role is BoundRole.Consumer)
            {
                return;
            }

            if (this.role is BoundRole.TestHost)
            {
                if (!this.serviceProvider.IsBoundTo(serviceProvider))
                {
                    throw new InvalidOperationException(
                        "The Allure.TestingPlatform runtime has already been bound to another test host."
                    );
                }

                return;
            }

            switch (this.runtimeState)
            {
                case RuntimeState.Created:
                    this.PrepareCore(serviceProvider, BoundRole.TestHost);
                    return;
                case RuntimeState.Prepared:
                    this.role = BoundRole.TestHost;
                    this.serviceProvider.SetTarget(serviceProvider);
                    return;
                case RuntimeState.Built:
                    throw new InvalidOperationException(
                        "The Allure.TestingPlatform runtime has already been constructed."
                    );
                default:
                    // Failed and Disposed were handled by ThrowIfUnavailable.
                    throw new InvalidOperationException(
                        $"Unexpected runtime coordinator state: {this.runtimeState}."
                    );
            }
        }
    }

    public ITestingPlatformRequestBinding BindConsumer(IServiceProvider serviceProvider)
    {
        if (serviceProvider is null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        lock (this.gate)
        {
            this.ThrowIfUnavailable();

            if (this.requestState is RequestBindingState.Active)
            {
                throw new InvalidOperationException(
                    "Another Microsoft Testing Platform request is active. "
                        + "Parallel requests are not supported."
                );
            }

            switch (this.runtimeState)
            {
                case RuntimeState.Created:
                    this.PrepareCore(serviceProvider, BoundRole.Consumer);
                    return new TestingPlatformRequestBinding(this, serviceProvider);
                case RuntimeState.Prepared or RuntimeState.Built:
                    this.role = BoundRole.Consumer;
                    this.serviceProvider.SetTarget(serviceProvider);
                    return new TestingPlatformRequestBinding(this, serviceProvider);
                default:
                    // Failed and Disposed were handled by ThrowIfUnavailable.
                    throw new InvalidOperationException(
                        $"Unexpected runtime coordinator state: {this.runtimeState}."
                    );
            }
        }
    }

    public void ActivateRequest(TestingPlatformRequestBinding binding)
    {
        lock (this.gate)
        {
            this.ThrowIfUnavailable();

            if (!this.serviceProvider.IsBoundTo(binding.ServiceProvider))
            {
                throw new InvalidOperationException(
                    "The request binding no longer owns the test-host service provider."
                );
            }

            switch (this.requestState)
            {
                case RequestBindingState.Unbound or RequestBindingState.Completed:
                    this.messageChannel.SetTarget(binding.ServiceProvider.GetMessageBus());
                    this.currentRequest = binding;
                    this.requestState = RequestBindingState.Active;
                    return;

                case RequestBindingState.Active:
                    if (!ReferenceEquals(this.currentRequest, binding))
                    {
                        throw new InvalidOperationException(
                            "Another Microsoft Testing Platform request is already active. "
                                + "Parallel requests are not supported."
                        );
                    }
                    return;

                default:
                    throw new InvalidOperationException(
                        $"Unexpected request binding state {this.requestState}"
                    );
            }
        }
    }

    public void ReleaseRequest(ITestingPlatformRequestBinding binding)
    {
        lock (this.gate)
        {
            // Can do even after the coordinator is disposed.
            if (!ReferenceEquals(this.currentRequest, binding))
            {
                return;
            }

            switch (this.requestState)
            {
                case RequestBindingState.Active:
                    this.messageChannel.ClearTarget();
                    this.requestState = RequestBindingState.Completed;
                    return;

                case RequestBindingState.Completed:
                case RequestBindingState.Unbound:
                    return;
            }
        }
    }

    public void DisposeRequestBinding(ITestingPlatformRequestBinding binding)
    {
        lock (this.gate)
        {
            if (!ReferenceEquals(this.currentRequest, binding))
            {
                return;
            }

            this.messageChannel.ClearTarget();
            this.currentRequest = null;
            this.requestState = RequestBindingState.Unbound;
        }
    }

    public void Dispose()
    {
        IAllureRuntimeRegistration<TRuntime>? registration;

        lock (this.gate)
        {
            if (this.runtimeState is RuntimeState.Disposed)
            {
                return;
            }

            this.messageChannel.ClearTarget();

            this.currentRequest = null;
            this.requestState = RequestBindingState.Unbound;

            registration = this.runtimeRegistration;
            this.runtimeRegistration = null;
            this.runtimeState = RuntimeState.Disposed;
        }

        try
        {
            registration?.Dispose();
        }
        finally
        {
            this.messageChannel.Dispose();
            this.serviceProvider.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        IAllureRuntimeRegistration<TRuntime>? registration;

        lock (this.gate)
        {
            if (this.runtimeState is RuntimeState.Disposed)
            {
                return;
            }

            this.messageChannel.ClearTarget();

            this.currentRequest = null;
            this.requestState = RequestBindingState.Unbound;

            registration = this.runtimeRegistration;
            this.runtimeRegistration = null;
            this.runtimeState = RuntimeState.Disposed;
        }

        try
        {
            if (registration is not null)
            {
                await registration.DisposeAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            this.messageChannel.Dispose();
            this.serviceProvider.Dispose();
        }
    }

    void PrepareCore(IServiceProvider serviceProvider, BoundRole role)
    {
        try
        {
            this.serviceProvider.SetTarget(serviceProvider);
            this.registrationPlan = this.runtimeBuilder.Prepare(
                (context) => this.registration(context, this)
            );
            this.runtimeState = RuntimeState.Prepared;
            this.role = role;
        }
        catch (Exception exception)
        {
            this.failure = exception;
            this.runtimeState = RuntimeState.Failed;
            throw;
        }
    }

    void ThrowIfUnprepared()
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
        if (this.runtimeState is RuntimeState.Disposed)
        {
            throw new ObjectDisposedException(this.GetType().FullName);
        }

        if (this.runtimeState is RuntimeState.Failed)
        {
            throw new InvalidOperationException(
                "The Allure.TestingPlatform runtime registration has failed.",
                this.failure
            );
        }
    }

    public ITestExecutionCoordinator CreateTestExecutionCoordinator()
    {
        lock (this.gate)
        {
            this.ThrowIfUnprepared();

            return this.internalServiceProvider.CreateTestExecutionCoordinator(
                this.registrationPlan!.Configuration
            );
        }
    }

    public void ConfigureEndpoint(IAllureEndpointRegistrationContext context)
    {
        lock (this.gate)
        {
            this.ThrowIfUnprepared();

            this.internalServiceProvider.ConfigureEndpoint(
                this.registrationPlan!.Configuration,
                context
            );
        }
    }

    enum BoundRole
    {
        None,

        Controller,

        TestHost,

        Consumer,
    }

    enum RuntimeState
    {
        Created,

        Prepared,

        Built,

        Failed,

        Disposed,
    }

    enum RequestBindingState
    {
        Unbound,

        Active,

        Completed,
    }
}

internal static class AllureTestingPlatformRuntimeRegistration
{
    public static AllureTestingPlatformRuntimeRegistration<
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
        Action<
            TIntegrationContext,
            AllureTestingPlatformRuntimeRegistration<
                TConfiguration,
                TRuntime,
                TIntegrationContext
            >
        > registration,
        InternalServiceProvider<TConfiguration> coordinatorProvider
    )
        where TConfiguration : AllureTestingPlatformConfiguration, new()
        where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
        where TIntegrationContext : IAllureTestingPlatformIntegrationContextBase<
            TConfiguration,
            TRuntime
        >
    =>
        new(runtimeName, sessionFactory, registration, coordinatorProvider);
}
