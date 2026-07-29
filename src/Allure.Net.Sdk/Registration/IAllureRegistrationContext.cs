using System;
using Allure.Abstractions;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures services shared by Allure runtimes and endpoints.
/// </summary>
public interface IAllureRegistrationContext
{
    /// <summary>
    /// Replaces the rule-based parameter serializer with a custom serializer.
    /// </summary>
    void UseParameterSerializer(Func<IAllureParameterSerializer> serializerFactory);

    /// <summary>
    /// Configures the rules used by the default parameter serializer.
    /// </summary>
    void ConfigureSerialization(Action<IParameterSerializationRulesContext> registration);
}
