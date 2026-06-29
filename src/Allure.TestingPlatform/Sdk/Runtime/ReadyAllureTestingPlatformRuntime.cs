using System;
using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Sdk.Correlation;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

public sealed record class ReadyAllureTestingPlatformRuntime(
    AllureTestingPlatformRegistrationMode Mode,
    ILogger Logger,
    AllureConfiguration Configuration,
    ICorrelationStrategy CorrelationStrategy,
    IAllureResultsWriter Writer,
    ImmutableDictionary<Type, ITypeFormatter> TypeFormatters,
    AllureLifecycle Lifecycle
) : ConfiguredAllureTestingPlatformRuntime(
    Mode: Mode,
    State: AllureTestingPlatformRuntimeState.Ready,
    Logger: Logger,
    Configuration: Configuration,
    IsEnabled: true
);
