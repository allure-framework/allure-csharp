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
    IAllureTestingPlatformRuntimeProvider runtimeProvider
) :
    IExtension
{
    public string Uid => uid;

    public string Version { get; } = TestingPlatformFunctions.CurrentPackageVersion;

    public string DisplayName => displayName;

    public string Description => description;

    public virtual Task<bool> IsEnabledAsync() =>
        runtimeProvider is
        {
            Value:
            {
                State: not AllureTestingPlatformRuntimeState.NotInitialized,
                IsEnabled: var isEnabled,
            },
        }
            ? Task.FromResult(isEnabled)
            : throw new InvalidOperationException(
                "Unexpected error: Allure.TestingPlatform is misconfigured."
            );

    protected ILogger Logger => ConfiguredRuntime.Logger;

    protected AllureConfiguration Configuration => ConfiguredRuntime.Configuration;

    protected IAllureResultsWriter Writer => this.ReadyRuntime.Writer;

    protected ImmutableDictionary<Type, ITypeFormatter> TypeFormatters =>
        this.ReadyRuntime.TypeFormatters;

    protected AllureLifecycle Lifecycle => this.ReadyRuntime.Lifecycle;

    protected ICorrelationSource CorrelationSource => this.ReadyRuntime.CorrelationSource;

    protected ConfiguredAllureTestingPlatformRuntime ConfiguredRuntime =>
        runtimeProvider is { Value: ConfiguredAllureTestingPlatformRuntime configuredRuntime }
            ? configuredRuntime
            : throw new InvalidOperationException(
                "Allure configuration is unavailable. Check if Allure.TestingPlatform is initialized correctly."
            );

    protected ReadyAllureTestingPlatformRuntime ReadyRuntime =>
        runtimeProvider is { Value: ReadyAllureTestingPlatformRuntime readyRuntime }
            ? readyRuntime
            : throw new InvalidOperationException(
                "Allure runtime is unavailable. Check if Allure.TestingPlatform is initialized correctly."
            );
}
