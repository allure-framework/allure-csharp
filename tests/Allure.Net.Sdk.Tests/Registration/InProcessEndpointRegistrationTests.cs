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
        var plan = builder.Prepare((ctx) =>
        {
            ctx.UseConfiguration(new AllureConfiguration());
            ctx.UseDestination(_ => destination);
            ctx.RegisterInProcessEndpoint(
                NewRouteId(),
                (_, endpoint) =>
                {
                    endpoint.SetAvailabilityPredicate(() => isAvailable);
                    endpoint.UseGlobalScopePredicate(_ => isInGlobalScope.Value);
                    endpoint.UseCurrentScopePredicate(_ => false);
                }
            );

        });
        using var _ = plan.Build();

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
        using var _ = targetBuilder.Prepare((ctx) =>
        {
            ctx.UseConfiguration(new AllureConfiguration());
            ctx.UseDestination(_ => targetDestination);
            ctx.RegisterInProcessEndpoint(
                targetRouteId,
                (_, endpoint) =>
                {
                    endpoint.UseGlobalScopePredicate(_ => isInGlobalScope.Value);
                    endpoint.UseCurrentScopePredicate(_ => false);
                }
            );

        }).Build();

        var suppressingBuilder = CreateBuilder();
        suppressingBuilder.Prepare((ctx) =>
        {
            ctx.UseConfiguration(new AllureConfiguration());
            ctx.UseDestination(_ => suppressingDestination);
            ctx.RegisterInProcessEndpoint(
                NewRouteId(),
                (_, endpoint) =>
                {
                    endpoint.UseGlobalScopePredicate(_ => isInGlobalScope.Value);
                    endpoint.UseCurrentScopePredicate(_ => false);
                    endpoint.SuppressRoutes(() => [targetRouteId]);
                }
            );
        }).Build();

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
        using var _ = builder.Prepare((ctx) =>
        {
            ctx.UseConfiguration(new AllureConfiguration());
            ctx.RegisterInProcessEndpoint(
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
        }).Build();

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
    public async Task ShouldExposeIntegrationSpecificRuntimeToEndpointRegistration()
    {
        var service = new object();
        var observedService = default(object);
        var builder = new RuntimeWithServiceBuilder("test", service);

        using var _ = builder.Prepare(ctx =>
            ctx.RegisterInProcessEndpoint(
                NewRouteId(),
                (runtime, endpoint) =>
                {
                    endpoint.UseCurrentScopePredicate(_ => false);
                    endpoint.UseGlobalScopePredicate(_ => false);
                    endpoint.SetAvailabilityPredicate(_ => false);
                    observedService = runtime.Service;
                }
            )
        ).Build();

        await Assert.That(observedService).IsSameReferenceAs(service);
    }

    static AllureRuntimeBuilder<AllureConfiguration> CreateBuilder() =>
        new("endpoint-registration-tests");

    static string NewRouteId() => $"endpoint-registration-{Guid.NewGuid():N}";

    sealed class RuntimeWithService(
        RuntimeCreationArguments<AllureConfiguration> args,
        object service
    ) : AllureRuntime<AllureConfiguration>(args)
    {
        public object Service { get; } = service;
    }

    sealed class RuntimeWithServiceRegistrationSession(object service) :
        AllureRuntimeRegistrationSession<AllureConfiguration, RuntimeWithService>
    {
        protected override RuntimeWithService CreateRuntime(RuntimeCreationArguments<AllureConfiguration> args)
        {
            return new (args, service);
        }
    }

    sealed class RuntimeWithServiceBuilder(
        string runtimeName,
        object service
    ) :
        AllureRuntimeBuilder<AllureConfiguration, RuntimeWithService>(
            runtimeName,
            () => new RuntimeWithServiceRegistrationSession(service)
        );
}
