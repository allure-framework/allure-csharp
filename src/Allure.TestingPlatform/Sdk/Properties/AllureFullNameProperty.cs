using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureFullNameProperty(string fullName) : IAllureProperty<TestResult>
{
    public string FullName { get; } = fullName;

    public void Apply(ReadyAllureTestingPlatform _, TestResult obj)
    {
        obj.fullName = this.FullName;
    }
}