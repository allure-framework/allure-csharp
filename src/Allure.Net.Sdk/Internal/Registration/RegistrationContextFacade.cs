using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

class RegistrationContextFacade<TConfiguration, TRuntime>(
    IAllureRuntimeIntegrationContext<
        TConfiguration,
        TRuntime,
        IAllureRuntimeRegistrationContext<TConfiguration>
    > integrationContext
) :
    IAllureRuntimeRegistrationContext<TConfiguration>

    where TConfiguration : AllureConfiguration, new()
    where TRuntime : IAllureRuntime<TConfiguration>
{
    public void ConfigureSerialization(
        Action<TConfiguration, IParameterSerializationRulesContext> registration
    ) =>
        integrationContext.ConfigureSerialization(registration);

    public void ConfigureSerialization(Action<IParameterSerializationRulesContext> registration) =>
        integrationContext.ConfigureSerialization(registration);

    public void TransformConfiguration(
        Func<TrackedConfiguration<TConfiguration>, TrackedConfiguration<TConfiguration>> transformation
    ) =>
        integrationContext.TransformConfiguration(transformation);

    public void UseConfigurationSources(
        Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> sourcesFactory
    ) =>
        integrationContext.UseConfigurationSources(sourcesFactory);

    public void UseDestination(Func<TConfiguration, IAllureResultsDestination> destinationFactory) =>
        integrationContext.UseDestination(destinationFactory);

    public void UseParameterSerializer(Func<TConfiguration, IAllureParameterSerializer> serializerFactory) =>
        integrationContext.UseParameterSerializer(serializerFactory);

    public void UseParameterSerializer(Func<IAllureParameterSerializer> serializerFactory) =>
        integrationContext.UseParameterSerializer(serializerFactory);
}

sealed class RegistrationContextFacade(
    IAllureRuntimeIntegrationContext integrationContext
) :
    RegistrationContextFacade<AllureConfiguration, IAllureRuntime>(integrationContext),
    IAllureRuntimeRegistrationContext;
