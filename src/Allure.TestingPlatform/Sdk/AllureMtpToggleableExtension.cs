using System;
using System.Threading.Tasks;
using Allure.TestingPlatform.Implementation;
using Microsoft.Testing.Platform.Services;

namespace Allure.TestingPlatform.Sdk;

public abstract class AllureMtpToggleableExtension(
    string uid,
    string displayName,
    string description,
    IServiceProvider serviceProvider
) : AllureMtpExtensionBase(uid, displayName, description)
{
    protected IServiceProvider ServiceProvider => serviceProvider;

    public bool IsEnabled() => AllureCliOptionsProvider.IsAllureEnabled(
        this.ServiceProvider.GetCommandLineOptions()
    );

    public override Task<bool> IsEnabledAsync() => Task.FromResult(
        this.IsEnabled()
    );
}