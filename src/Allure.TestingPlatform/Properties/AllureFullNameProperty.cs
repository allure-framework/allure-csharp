using Allure.Net.Commons;

namespace Allure.TestingPlatform.Properties;

public sealed class AllureFullNameProperty(string fullName) : IAllureProperty<TestResult>
{
    public string FullName { get; } = fullName;

    public void Apply(IAllureInfrastructure _, TestResult obj)
    {
        obj.fullName = this.FullName;
    }
}