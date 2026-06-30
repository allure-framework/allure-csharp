using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets the full name of an Allure test result.
/// </summary>
public sealed class AllureFullNameProperty(string fullName) : IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the full name to set.
    /// </summary>
    public string FullName { get; } = fullName;

    /// <inheritdoc />
    public void Apply(LiveAllureTestingPlatformRuntime _, TestResult target)
    {
        target.fullName = this.FullName;
    }
}
