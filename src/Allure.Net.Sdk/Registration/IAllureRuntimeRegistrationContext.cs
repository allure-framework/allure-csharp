using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Results;

namespace Allure.Sdk.Registration;

public interface IAllureRuntimeRegistrationContext<TConfiguration> :
    IAllureRegistrationContext
    where TConfiguration : AllureConfiguration
{
    void UseConfigurationSources(
        Func<IEnumerable<IAllureConfigurationSource<TConfiguration>>> sourcesFactory
    );

    void ConfigureSerialization(Action<TConfiguration, IParameterSerializationRulesContext> registration);

    void UseParameterSerializer(
        Func<TConfiguration, IAllureParameterSerializer> serializerFactory
    );

    void UseDestination(Func<TConfiguration, IAllureResultsDestination> destinationFactory);
}
