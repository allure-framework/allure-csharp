using System;
using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime.AdapterState;

public sealed record class ReadyAllureTestingPlatform(
    AllureTestingPlatformRegistrationMode Mode,
    ILogger Logger,
    AllureConfiguration Configuration,
    ICorrelationSource CorrelationSource,
    IAllureResultsWriter Writer,
    ImmutableDictionary<Type, ITypeFormatter> TypeFormatters,
    AllureLifecycle Lifecycle
) : ConfiguredAllureTestingPlatform(
    Mode: Mode,
    State: AllureTestingPlatformState.Ready,
    Logger: Logger,
    Configuration: Configuration,
    IsEnabled: true
);
