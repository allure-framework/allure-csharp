using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureFullNameProperty(string fullName) : IAllureProperty<TestResult>
{
    public string FullName { get; } = fullName;

    public void Apply(LiveAllureTestingPlatformRuntime _, TestResult target)
    {
        target.fullName = this.FullName;
    }
}