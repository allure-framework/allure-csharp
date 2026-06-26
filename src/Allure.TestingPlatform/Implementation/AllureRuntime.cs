using System;
using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Implementation;

public class AllureRuntime(
    AllureConfiguration configuration,
    ILogger logger,
    ICorrelationService correlationService,
    IAllureResultsWriter writer,
    ImmutableDictionary<Type, ITypeFormatter> typeFormatters,
    AllureLifecycle lifecycle
) :
    IAllureRuntime
{
    public AllureConfiguration Config => configuration;

    public ILogger Logger => logger;

    public ICorrelationService CorrelationService => correlationService;

    public IAllureResultsWriter Writer => writer;

    public ImmutableDictionary<Type, ITypeFormatter> TypeFormatters => typeFormatters;

    public AllureLifecycle Lifecycle => lifecycle;
}