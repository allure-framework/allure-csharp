using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

public abstract class AllureTestingPlatformExtension(
    string uid,
    string displayName,
    string description,
    IAllureTestingPlatformServiceProvider allureTestingPlatformStateProvider
) :
    IExtension
{
    public string Uid => uid;

    public string Version { get; } = TestingPlatformFunctions.CurrentPackageVersion;

    public string DisplayName => displayName;

    public string Description => description;

    public virtual Task<bool> IsEnabledAsync() =>
        allureTestingPlatformStateProvider is
        {
            Value:
            {
                State: not AllureTestingPlatformState.NotInitialized,
                IsEnabled: var isEnabled,
            },
        }
            ? Task.FromResult(isEnabled)
            : throw new InvalidOperationException(
                "Unexpected error: Allure.TestingPlatform is misconfigured."
            );

    protected ILogger Logger => ConfiguredState.Logger;

    protected AllureConfiguration Configuration => ConfiguredState.Configuration;

    protected IAllureResultsWriter Writer => this.ReadyState.Writer;

    protected ImmutableDictionary<Type, ITypeFormatter> TypeFormatters =>
        this.ReadyState.TypeFormatters;

    protected AllureLifecycle Lifecycle => this.ReadyState.Lifecycle;

    protected ICorrelationSource CorrelationSource => this.ReadyState.CorrelationSource;

    protected ConfiguredAllureTestingPlatform ConfiguredState =>
        allureTestingPlatformStateProvider is { Value: ConfiguredAllureTestingPlatform configuredState }
            ? configuredState
            : throw new InvalidOperationException(
                "Allure configuration is unavailable. Check if Allure.TestingPlatform is initialized correctly."
            );

    protected ReadyAllureTestingPlatform ReadyState =>
        allureTestingPlatformStateProvider is { Value: ReadyAllureTestingPlatform readyState }
            ? readyState
            : throw new InvalidOperationException(
                "Allure runtime is unavailable. Check if Allure.TestingPlatform is initialized correctly."
            );
}
