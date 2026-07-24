namespace Allure.Sdk.Configuration;

public interface IAllureConfigurationSource<out TConfig>
    where TConfig : AllureConfiguration
{
    string Name { get; }

    bool CanLoad { get; }

    TConfig LoadConfiguration();
}
