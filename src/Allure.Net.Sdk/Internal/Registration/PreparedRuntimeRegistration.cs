using System;
using System.Collections.Generic;
using Allure.Runtime;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

internal class PreparedRuntimeRegistration<TConfiguration, TRuntime>(
    string runtimeName,
    TConfiguration configuration,
    AllureRuntimeRegistrationSnapshot<TConfiguration, TRuntime> registrationSnapshot,
    Func<RuntimeCreationArguments<TConfiguration>, TRuntime> runtimeFactory
) :
    IPreparedRuntimeRegistration<TConfiguration, TRuntime>

    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
{
    public TConfiguration Configuration => configuration;

    public IAllureRuntimeRegistration<TRuntime> Build()
    {
        var parameterSerializer = registrationSnapshot.SerializerFactory(configuration);
        var destination = registrationSnapshot.DestinationFactory(configuration);

        var constructionRuntimeReference =
            new LateBoundReference<IAllureRuntime<TConfiguration>>();

        var serviceCreationContext =
            new RuntimeServiceCreationContext<TConfiguration>(
                configuration,
                constructionRuntimeReference
            );

        var context = registrationSnapshot.ContextFactory(serviceCreationContext);
        var lifecycleApi = registrationSnapshot.LifecycleApiFactory(serviceCreationContext);
        var modelApi = registrationSnapshot.ModelApiFactory(serviceCreationContext);
        var testPlan = registrationSnapshot.TestPlanFactory(configuration);

        RuntimeCreationArguments<TConfiguration> runtimeCreationArguments = new(
            Configuration: configuration,
            ParameterSerializer: parameterSerializer,
            Destination: destination,
            Context: context,
            LifecycleApi: lifecycleApi,
            ModelApi: modelApi,
            TestPlan: testPlan
        );

        var runtime = runtimeFactory(runtimeCreationArguments);

        IDisposable? endpointRegistration = null;

        try
        {
            constructionRuntimeReference.Bind(runtime);

            if (registrationSnapshot.EndpointRegistration is var (routeId, routeRegistration))
            {
                var routeBuilder = new AllureInProcessRouteBuilder<TConfiguration, TRuntime>(
                    runtimeName,
                    routeId,
                    runtime,
                    registrationSnapshot.UseRuleBasedSerializer,
                    registrationSnapshot.RuleBasedSerializerRegistrations,
                    routeRegistration
                );
                var route = routeBuilder.Build();

                endpointRegistration = AllureRuntimeRouter.Install(route);
            }

            return new AllureRuntimeRegistration<TRuntime>(runtime, endpointRegistration);
        }
        catch (Exception buildException)
        {
            CleanupRuntimeRegistration(runtime, endpointRegistration, buildException);
            throw;
        }
    }

    static void CleanupRuntimeRegistration(
        TRuntime runtime,
        IDisposable? endpointRegistration,
        Exception buildException
    )
    {
        var cleanupExceptions = new List<Exception>();

        try
        {
            endpointRegistration?.Dispose();
        }
        catch (Exception exception)
        {
            cleanupExceptions.Add(exception);
        }

        try
        {
            DisposeRuntimeAfterFailedBuild(runtime);
        }
        catch (Exception exception)
        {
            cleanupExceptions.Add(exception);
        }

        if (cleanupExceptions.Count > 0)
        {
            throw new AggregateException(
                "Runtime construction failed and cleanup also failed.",
                [buildException, .. cleanupExceptions]
            );
        }
    }

    static void DisposeRuntimeAfterFailedBuild(TRuntime runtime)
    {
        if (runtime is IDisposable disposable)
        {
            disposable.Dispose();
            return;
        }

        if (runtime is IAsyncDisposable asyncDisposable)
        {
            asyncDisposable
                .DisposeAsync()
                .AsTask()
                .GetAwaiter()
                .GetResult();
        }
    }
}
