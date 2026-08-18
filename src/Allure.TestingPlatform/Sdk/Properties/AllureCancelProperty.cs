using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Cancels the test result precluding it from being written to the results directory.
/// </summary>
public sealed class AllureCancelProperty() : IAllureProperty<TestResult>
{
    /// <inheritdoc />
    public void Apply(
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime,
        TestResult target
    )
    {
        target.Labels.Add(new(){ Name = CANCEL_LABEL_NAME, Value = "true" });
    }

    /// <summary>
    /// The name of the marker label that signals the test result was cancelled.
    /// </summary>
    public const string CANCEL_LABEL_NAME = "__allure_cancelled__";
}
