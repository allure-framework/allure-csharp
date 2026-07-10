using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Cancels the test result precluding it from being written to the results directory.
/// </summary>
public sealed class AllureCancelProperty() : IAllureProperty<TestResult>
{
    /// <inheritdoc />
    public void Apply(LiveAllureTestingPlatformRuntime _, TestResult target)
    {
        target.labels.Add(new(){ name = CANCEL_LABEL_NAME, value = "true" });
    }

    /// <summary>
    /// The name of the marker label that signals the test result was cancelled.
    /// </summary>
    public const string CANCEL_LABEL_NAME = "__allure_cancelled__";
}
