using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;

namespace Allure.TestingPlatform.Registration;

public class AllureInfrastructure(
    AllureConfiguration config,
    IAllureResultsWriter writer,
    AllureLifecycle lifecycle,
    IReadOnlyDictionary<Type, ITypeFormatter> typeFormatters
)
    : IAllureInfrastructure
{
    public AllureConfiguration Config { get; } = config;
    public IAllureResultsWriter Writer { get; } = writer;
    public AllureLifecycle Lifecycle { get; } = lifecycle;
    public IReadOnlyDictionary<Type, ITypeFormatter> TypeFormatters { get; } = typeFormatters;
}