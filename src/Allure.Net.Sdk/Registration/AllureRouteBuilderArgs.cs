using System;
using System.Collections.Generic;
using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Carries the components resolved by a runtime builder into an in-process
/// route builder.
/// </summary>
/// <remarks>
/// Instances are created by the SDK and supplied to
/// <c>CreateRouteBuilder</c> implementations.
/// </remarks>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public class AllureRouteBuilderArgs<TConfiguration, TRuntime>
    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
{
    internal string RuntimeName { get; }
    internal string RouteId { get; }
    internal TRuntime Runtime { get; }
    internal bool UseRuleBasedSerializer { get; }
    internal IEnumerable<Action<TConfiguration, IParameterSerializationRulesContext>> RuleBasedSerializerRegistrations { get; }

    internal AllureRouteBuilderArgs(
        string runtimeName,
        string routeId,
        TRuntime runtime,
        bool useRuleBasedSerializer,
        IEnumerable<Action<TConfiguration, IParameterSerializationRulesContext>> ruleBasedSerializerRegistrations
    )
    {
        this.RuntimeName = runtimeName;
        this.RouteId = routeId;
        this.Runtime = runtime;
        this.UseRuleBasedSerializer = useRuleBasedSerializer;
        this.RuleBasedSerializerRegistrations = ruleBasedSerializerRegistrations;
    }
}
