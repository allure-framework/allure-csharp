using System;
using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Sdk.Correlation;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Represents a started runtime with all dependencies needed to write Allure results.
/// </summary>
/// <param name="Mode">The registration mode.</param>
/// <param name="Logger">The resolved logger.</param>
/// <param name="Configuration">The resolved Allure configuration.</param>
/// <param name="CorrelationStrategy">The resolved correlation strategy.</param>
/// <param name="Writer">The resolved Allure results writer.</param>
/// <param name="TypeFormatters">The resolved type formatters.</param>
/// <param name="Lifecycle">The resolved Allure lifecycle.</param>
public sealed record class LiveAllureTestingPlatformRuntime(
    AllureTestingPlatformRegistrationMode Mode,
    ILogger Logger,
    AllureConfiguration Configuration,
    ICorrelationStrategy CorrelationStrategy,
    IAllureResultsWriter Writer,
    ImmutableDictionary<Type, ITypeFormatter> TypeFormatters,
    AllureLifecycle Lifecycle
) : ConfiguredAllureTestingPlatformRuntime(
    Mode: Mode,
    Phase: AllureTestingPlatformRuntimePhase.Live,
    Logger: Logger,
    Configuration: Configuration,
    IsEnabled: true
);
