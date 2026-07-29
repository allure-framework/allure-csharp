using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
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

    static TestRuntimeBuilder<AllureConfiguration> CreateBuilder() =>
        new("endpoint-registration-tests");

    static string NewRouteId() => $"endpoint-registration-{Guid.NewGuid():N}";
}
