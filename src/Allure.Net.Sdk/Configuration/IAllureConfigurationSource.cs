namespace Allure.Sdk.Configuration;

public interface IAllureConfigurationSource<out TConfiguration>
    where TConfiguration : AllureConfiguration
{
    string Name { get; }

    bool CanLoad { get; }

    TConfiguration LoadConfiguration();
}
