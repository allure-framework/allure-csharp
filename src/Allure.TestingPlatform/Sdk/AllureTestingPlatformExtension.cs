using System;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Functions;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.Services;

namespace Allure.TestingPlatform.Sdk;

public abstract class AllureTestingPlatformExtension(
    string uid,
    string displayName,
    string description,
    IServiceProvider serviceProvider,
    IAllureExtensionSettings settings
) :
    IExtension
{
    readonly Lazy<IAllureRuntimeProvider> allureRuntimeProvider = new(
        serviceProvider.GetRequiredService<IAllureRuntimeProvider>
    );

    public AllureTestingPlatformExtension(
        string uid,
        string displayName,
        string description,
        IServiceProvider serviceProvider
    ) : this(
        uid,
        displayName,
        description,
        serviceProvider,
        serviceProvider.AllureExtensionSettings
    )
    {
    }

    public string Uid => uid;

    public string Version { get; } = TestingPlatformFunctions.CurrentPackageVersion;

    public string DisplayName => displayName;

    public string Description => description;

    public Task<bool> IsEnabledAsync() => Task.FromResult(settings.IsEnabled);

    protected IServiceProvider ServiceProvider => serviceProvider;

    protected IAllureRuntimeProvider RuntimeProvider => this.allureRuntimeProvider.Value;

    protected IAllureRuntime Allure => this.RuntimeProvider.Runtime;

    protected ILogger Logger => this.Allure.Logger;

    protected AllureConfiguration Config => this.Allure.Config;

    protected IAllureResultsWriter Writer => this.Allure.Writer;

    protected AllureLifecycle Lifecycle => this.Allure.Lifecycle;
}
