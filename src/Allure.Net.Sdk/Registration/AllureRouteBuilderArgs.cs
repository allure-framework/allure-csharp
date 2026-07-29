using System;
using System.Collections.Generic;
using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public sealed class AllureRouteBuilderArgs<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    internal string RuntimeName { get; }
    internal string RouteId { get; }
    internal IAllureRuntime<TConfiguration> Runtime { get; }
    internal bool UseRuleBasedSerializer { get; }
    internal IEnumerable<Action<TConfiguration, IParameterSerializationRulesContext>> RuleBasedSerializerRegistrations { get; }

    internal AllureRouteBuilderArgs(
        string runtimeName,
        string routeId,
        IAllureRuntime<TConfiguration> runtime,
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
