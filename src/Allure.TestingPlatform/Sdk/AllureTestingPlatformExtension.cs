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

public abstract class AllureTestingPlatformExtension(
    string uid,
    string displayName,
    string description,
    IAllureTestingPlatformRuntimeReference runtimeReference
) :
    IExtension
{
    public string Uid => uid;

    public string Version { get; } = TestingPlatformFunctions.CurrentPackageVersion;

    public string DisplayName => displayName;

    public string Description => description;

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

    protected ILogger Logger => ConfiguredRuntime.Logger;

    protected AllureConfiguration Configuration => ConfiguredRuntime.Configuration;

    protected IAllureResultsWriter Writer => this.LiveRuntime.Writer;

    protected ImmutableDictionary<Type, ITypeFormatter> TypeFormatters =>
        this.LiveRuntime.TypeFormatters;

    protected AllureLifecycle Lifecycle => this.LiveRuntime.Lifecycle;

    protected ICorrelationStrategy CorrelationStrategy => this.LiveRuntime.CorrelationStrategy;

    protected ConfiguredAllureTestingPlatformRuntime ConfiguredRuntime =>
        runtimeReference is { CurrentRuntime: ConfiguredAllureTestingPlatformRuntime configuredRuntime }
            ? configuredRuntime
            : throw new InvalidOperationException(
                "Allure configuration is unavailable. Check if Allure.TestingPlatform is initialized correctly."
            );

    protected LiveAllureTestingPlatformRuntime LiveRuntime =>
        runtimeReference is { CurrentRuntime: LiveAllureTestingPlatformRuntime liveRuntime }
            ? liveRuntime
            : throw new InvalidOperationException(
                "Allure runtime is unavailable. Check if Allure.TestingPlatform is initialized correctly."
            );
}
