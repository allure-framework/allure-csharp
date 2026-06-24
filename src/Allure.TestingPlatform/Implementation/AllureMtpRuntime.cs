using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Implementation;

public class AllureMtpRuntime(
    AllureConfiguration config,
    ILogger logger,
    bool isEnabled,
    ICorrelationService correlationService,
    IAllureResultsWriter writer,
    AllureLifecycle lifecycle,
    IReadOnlyDictionary<Type, ITypeFormatter> typeFormatters
)
    : IAllureRuntime
{
    public AllureConfiguration Config { get; } = config;
    public ILogger Logger { get; } = logger;
    public bool IsEnabled { get; } = isEnabled;
    public ICorrelationService CorrelationService { get; } = correlationService;
    public IAllureResultsWriter Writer { get; } = writer;
    public AllureLifecycle Lifecycle { get; } = lifecycle;
    public IReadOnlyDictionary<Type, ITypeFormatter> TypeFormatters { get; } = typeFormatters;
}