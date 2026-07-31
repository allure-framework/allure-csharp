using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.Registration;

public class InProcessEndpointRegistrationTests
{
    [Test]
    public async Task ShouldApplyAvailabilityAndGlobalScopePredicatesWhenCallingFacade()
    {
        var isInGlobalScope = new AsyncLocal<bool>();
        var isAvailable = false;
        var destination = new InMemoryResultsDestination();
        var builder = CreateBuilder();
        builder.UseConfiguration(new AllureConfiguration());
        builder.UseDestination(_ => destination);
        builder.RegisterInProcessEndpoint(
            NewRouteId(),
            (_, endpoint) =>
            {
                endpoint.SetAvailabilityPredicate(() => isAvailable);
                endpoint.UseGlobalScopePredicate(_ => isInGlobalScope.Value);
                endpoint.UseCurrentScopePredicate(_ => false);
            }
        );
        builder.Build();

        isInGlobalScope.Value = true;
        AllureApi.AddGlobalAttachment("unavailable", new byte[] { 1 });

        isAvailable = true;
        AllureApi.AddGlobalAttachment("available", new byte[] { 2 });

        isInGlobalScope.Value = false;
        AllureApi.AddGlobalAttachment("out-of-scope", new byte[] { 3 });

        await Assert.That(destination.Globals.Count).IsEqualTo(1);
        await Assert.That(destination.Globals[0].Attachments[0].Name).IsEqualTo("available");
        isInGlobalScope.Value = false;
    }

    [Test]
    public async Task ShouldSuppressAnotherRouteWhenCallingFacade()
    {
        var isInGlobalScope = new AsyncLocal<bool>();
        var targetDestination = new InMemoryResultsDestination();
        var suppressingDestination = new InMemoryResultsDestination();
        var targetRouteId = NewRouteId();
        var targetBuilder = CreateBuilder();
        targetBuilder.UseConfiguration(new AllureConfiguration());
        targetBuilder.UseDestination(_ => targetDestination);
        targetBuilder.RegisterInProcessEndpoint(
            targetRouteId,
            (_, endpoint) =>
            {
                endpoint.UseGlobalScopePredicate(_ => isInGlobalScope.Value);
                endpoint.UseCurrentScopePredicate(_ => false);
            }
        );
        targetBuilder.Build();

        var suppressingBuilder = CreateBuilder();
        suppressingBuilder.UseConfiguration(new AllureConfiguration());
        suppressingBuilder.UseDestination(_ => suppressingDestination);
        suppressingBuilder.RegisterInProcessEndpoint(
            NewRouteId(),
            (_, endpoint) =>
            {
                endpoint.UseGlobalScopePredicate(_ => isInGlobalScope.Value);
                endpoint.UseCurrentScopePredicate(_ => false);
                endpoint.SuppressRoutes(() => [targetRouteId]);
            }
        );
        suppressingBuilder.Build();

        isInGlobalScope.Value = true;
        AllureApi.AddGlobalAttachment("suppressed", new byte[] { 1 });

        await Assert.That(targetDestination.Globals).IsEmpty();
        await Assert.That(suppressingDestination.Globals.Count).IsEqualTo(1);
        isInGlobalScope.Value = false;
    }

    [Test]
    public async Task ShouldUseConfiguredOperationsWhenCallingFacade()
    {
        var isInGlobalScope = new AsyncLocal<bool>();
        var operationsFactoryCalls = 0;
        var builder = CreateBuilder();
        var syncOperations = IAllureInProcessSyncOperations.Mock();
        var asyncOperations = IAllureInProcessAsyncOperations.Mock();
        builder.UseConfiguration(new AllureConfiguration());
        builder.RegisterInProcessEndpoint(
            NewRouteId(),
            (_, endpoint) =>
            {
                endpoint.UseGlobalScopePredicate(_ => isInGlobalScope.Value);
                endpoint.UseCurrentScopePredicate(_ => false);
                endpoint.UseOperations(_ =>
                {
                    operationsFactoryCalls++;
                    return new(syncOperations, asyncOperations);
                });
            }
        );
        builder.Build();

        isInGlobalScope.Value = true;
        AllureApi.AddGlobalAttachment("custom operations", new byte[] { 1 });

        await Assert.That(operationsFactoryCalls).IsEqualTo(1);
        await Assert.That(syncOperations.AddGlobalAttachment(
            "custom operations",
            IsNotNull<Stream>(),
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
        isInGlobalScope.Value = false;
    }

    [Test]
    public async Task ShouldExposeIntegrationSpecificRuntimeToRouteBuilder()
    {
        var service = new object();
        var builder = new RuntimeWithServiceBuilder(
            "endpoint-registration-tests",
            service
        );

        builder.RegisterInProcessEndpoint(NewRouteId(), (_, ctx) =>
        {
            ctx.SetAvailabilityPredicate((_) => false);
            ctx.UseCurrentScopePredicate((_) => false);
            ctx.UseGlobalScopePredicate((_) => false);
        });

        using var _ = builder.Build();

        await Assert.That(builder.ServiceObservedByRouteBuilder)
            .IsSameReferenceAs(service);
    }

    static TestRuntimeBuilder<AllureConfiguration> CreateBuilder() =>
        new("endpoint-registration-tests");

    static string NewRouteId() => $"endpoint-registration-{Guid.NewGuid():N}";

    sealed class RuntimeWithService(
        AllureConfiguration configuration,
        IAllureParameterSerializer parameterSerializer,
        IAllureResultsDestination resultsDestination,
        IAllureExecutionContext context,
        IAllureLifecycleApi lifecycleApi,
        IAllureModelApi modelApi,
        object service
    ) : AllureRuntime<AllureConfiguration>(
        configuration,
        parameterSerializer,
        resultsDestination,
        context,
        lifecycleApi,
        modelApi
    )
    {
        public object Service { get; } = service;
    }

    sealed class RuntimeWithServiceBuilder(
        string runtimeName,
        object service
    ) :
        AllureRuntimeBuilder<
            AllureConfiguration,
            IAllureRuntimeRegistrationContext<AllureConfiguration>,
            RecordingRuntimeHook<AllureConfiguration>,
            IAllureInProcessEndpointRegistrationContext<AllureConfiguration, RuntimeWithService>,
            RecordingEndpointHook<AllureConfiguration, RuntimeWithService>,
            RuntimeWithService
        >(runtimeName),
        IAllureRuntimeRegistrationContext<AllureConfiguration>
    {
        public object? ServiceObservedByRouteBuilder { get; private set; }

        protected override IAllureRuntimeRegistrationContext<AllureConfiguration>
            RegistrationContext => this;

        protected override RuntimeWithService CreateRuntimeInstance(
            AllureConfiguration configuration,
            IAllureParameterSerializer parameterSerializer,
            IAllureResultsDestination destination,
            IAllureExecutionContext context,
            IAllureLifecycleApi lifecycleApi,
            IAllureModelApi modelApi
        ) => new(
            configuration,
            parameterSerializer,
            destination,
            context,
            lifecycleApi,
            modelApi,
            service
        );

        protected override AllureInProcessRouteBuilder<
            AllureConfiguration,
            IAllureInProcessEndpointRegistrationContext<AllureConfiguration, RuntimeWithService>,
            RecordingEndpointHook<AllureConfiguration, RuntimeWithService>,
            RuntimeWithService
        > CreateRouteBuilder(
            AllureRouteBuilderArgs<
                AllureConfiguration,
                RuntimeWithService
            > args
        )
        {
            var builder = new RuntimeWithServiceRouteBuilder(args);
            this.ServiceObservedByRouteBuilder = builder.Service;
            return builder;
        }
    }

    sealed class RuntimeWithServiceRouteBuilder(
        AllureRouteBuilderArgs<
            AllureConfiguration,
            RuntimeWithService
        > args
    ) :
        AllureInProcessRouteBuilder<
            AllureConfiguration,
            IAllureInProcessEndpointRegistrationContext<AllureConfiguration, RuntimeWithService>,
            RecordingEndpointHook<AllureConfiguration, RuntimeWithService>,
            RuntimeWithService
        >(args)
    {
        public object Service => this.Runtime.Service;

        protected override IAllureInProcessEndpointRegistrationContext<
            AllureConfiguration,
            RuntimeWithService
        > RegistrationContext => this;
    }
}
