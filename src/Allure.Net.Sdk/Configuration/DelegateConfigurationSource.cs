using System;

namespace Allure.Sdk.Configuration;

public sealed class DelegateConfigurationSource<TConfiguration>(
    string name,
    Func<TConfiguration> factory
) :
    IAllureConfigurationSource<TConfiguration>

    where TConfiguration : AllureConfiguration
{
    public string Name => name;

    public bool CanLoad => true;

    public TConfiguration LoadConfiguration() => factory();
}

public static class DelegateConfigurationSource
{
    public static DelegateConfigurationSource<TConfiguration> Create<TConfiguration>(
        string name,
        Func<TConfiguration> configurationFactory
    )
        where TConfiguration : AllureConfiguration
    =>
        new(name, configurationFactory);
}
