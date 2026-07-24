using System;

namespace Allure.Sdk.Configuration;

public sealed class LambdaConfigurationSource<TConfiguration>(
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

public static class LambdaConfigurationSource
{
    public static LambdaConfigurationSource<TConfiguration> Create<TConfiguration>(
        string name,
        Func<TConfiguration> configurationFactory
    )
        where TConfiguration : AllureConfiguration
    =>
        new(name, configurationFactory);
}
