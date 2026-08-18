using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets the full name of an Allure test result.
/// </summary>
/// <param name="fullName">The full name to set.</param>
public sealed class AllureFullNameProperty(string fullName) : IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the full name to set.
    /// </summary>
    public string FullName { get; } = fullName;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TestResult target)
    {
        target.FullName = this.FullName;
    }
}
