using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Registration;

public class AllureInfrastructure(
    bool isEnabled,
    AllureConfiguration config,
    ICorrelationDefinition correlationDefinition,
    IAllureResultsWriter writer,
    AllureLifecycle lifecycle,
    IReadOnlyDictionary<Type, ITypeFormatter> typeFormatters
)
    : IAllureInfrastructure
{
    public bool IsEnabled { get; } = isEnabled;
    public AllureConfiguration Config { get; } = config;
    public ICorrelationDefinition CorrelationDefinition { get; } = correlationDefinition;
    public IAllureResultsWriter Writer { get; } = writer;
    public AllureLifecycle Lifecycle { get; } = lifecycle;
    public IReadOnlyDictionary<Type, ITypeFormatter> TypeFormatters { get; } = typeFormatters;
}