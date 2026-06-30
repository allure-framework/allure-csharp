using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk;

/// <summary>
/// Base class for Allure.TestingPlatform extensions.
/// </summary>
public abstract class AllureTestingPlatformExtension(
    string uid,
    string displayName,
    string description,
    IAllureTestingPlatformRuntimeReference runtimeReference
) :
    IExtension
{
    /// <inheritdoc />
    public string Uid => uid;

    /// <inheritdoc />
    public string Version { get; } = TestingPlatformFunctions.CurrentPackageVersion;

    /// <inheritdoc />
    public string DisplayName => displayName;

    /// <inheritdoc />
    public string Description => description;

    /// <inheritdoc />
    public virtual Task<bool> IsEnabledAsync() =>
        runtimeReference is
        {
            CurrentRuntime:
            {
                Phase: not AllureTestingPlatformRuntimePhase.NotInitialized,
                IsEnabled: var isEnabled,
            },
        }
            ? Task.FromResult(isEnabled)
            : throw new InvalidOperationException(
                "Unexpected error: Allure.TestingPlatform runtime is not configured."
            );

    /// <summary>
    /// Gets the configured runtime logger.
    /// </summary>
    protected ILogger Logger => ConfiguredRuntime.Logger;

    /// <summary>
    /// Gets the resolved Allure configuration.
    /// </summary>
    protected AllureConfiguration Configuration => ConfiguredRuntime.Configuration;

    /// <summary>
    /// Gets the resolved Allure results writer.
    /// </summary>
    protected IAllureResultsWriter Writer => this.LiveRuntime.Writer;

    /// <summary>
    /// Gets the resolved parameter type formatters.
    /// </summary>
    protected ImmutableDictionary<Type, ITypeFormatter> TypeFormatters =>
        this.LiveRuntime.TypeFormatters;

    /// <summary>
    /// Gets the resolved Allure lifecycle.
    /// </summary>
    protected AllureLifecycle Lifecycle => this.LiveRuntime.Lifecycle;

    /// <summary>
    /// Gets the resolved correlation strategy.
    /// </summary>
    protected ICorrelationStrategy CorrelationStrategy => this.LiveRuntime.CorrelationStrategy;

    /// <summary>
    /// Gets the configured runtime state.
    /// </summary>
    protected ConfiguredAllureTestingPlatformRuntime ConfiguredRuntime =>
        runtimeReference is { CurrentRuntime: ConfiguredAllureTestingPlatformRuntime configuredRuntime }
            ? configuredRuntime
            : throw new InvalidOperationException(
                "Allure configuration is unavailable. Check if Allure.TestingPlatform is initialized correctly."
            );

    /// <summary>
    /// Gets the live runtime state.
    /// </summary>
    protected LiveAllureTestingPlatformRuntime LiveRuntime =>
        runtimeReference is { CurrentRuntime: LiveAllureTestingPlatformRuntime liveRuntime }
            ? liveRuntime
            : throw new InvalidOperationException(
                "Allure runtime is unavailable. Check if Allure.TestingPlatform is initialized correctly."
            );
}
