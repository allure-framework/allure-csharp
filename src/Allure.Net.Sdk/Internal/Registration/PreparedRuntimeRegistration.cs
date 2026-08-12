using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Runtime;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

internal class PreparedRuntimeRegistration<TConfiguration, TRuntime>(
    string runtimeName,
    TConfiguration configuration,
    AllureRuntimeRegistrationSnapshot<TConfiguration, TRuntime> commonSnapshot,
    Func<RuntimeCreationArguments<TConfiguration>, TRuntime> runtimeFactory,
    Func<EndpointRouteCreationArguments<TConfiguration, TRuntime>, IAllureRuntimeRoute> routeFactory
) :
    IPreparedRuntimeRegistration<TConfiguration, TRuntime>

    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
{
    public TConfiguration Configuration => configuration;

    public IAllureRuntimeRegistration<TRuntime> Build()
    {
        var parameterSerializer = commonSnapshot.SerializerFactory(configuration);
        var destination = commonSnapshot.DestinationFactory(configuration);

        var constructionRuntimeReference =
            new LateBoundReference<IAllureRuntime<TConfiguration>>();

        var serviceCreationContext =
            new RuntimeServiceCreationContext<TConfiguration>(
                configuration,
                constructionRuntimeReference
            );

        var context = commonSnapshot.ContextFactory(serviceCreationContext);
        var lifecycleApi = commonSnapshot.LifecycleApiFactory(serviceCreationContext);
        var modelApi = commonSnapshot.ModelApiFactory(serviceCreationContext);

        RuntimeCreationArguments<TConfiguration> runtimeCreationArguments = new(
            Configuration: configuration,
            ParameterSerializer: parameterSerializer,
            Destination: destination,
            Context: context,
            LifecycleApi: lifecycleApi,
            ModelApi: modelApi
        );

        var runtime = runtimeFactory(runtimeCreationArguments);

        IDisposable? endpointRegistration = null;

        try
        {
            constructionRuntimeReference.Bind(runtime);

            if (commonSnapshot.EndpointRegistration is var (routeId, routeRegistration))
            {
                EndpointRouteCreationArguments<TConfiguration, TRuntime> endpointBuildArgs = new(
                    runtimeName,
                    routeId,
                    runtime,
                    commonSnapshot.UseRuleBasedSerializer,
                    commonSnapshot.RuleBasedSerializerRegistrations,
                    routeRegistration
                );

                var route = routeFactory(endpointBuildArgs);

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
