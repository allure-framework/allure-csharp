using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets or appends the expected results of a test in the HTML format.
/// </summary>
/// <param name="expectedResultsHtml">The expected results in the HTML format.</param>
public sealed class AllureExpectedResultsHtmlProperty(string expectedResultsHtml) :
    IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the expected results HTML text.
    /// </summary>
    public string ExpectedResultsHtml { get; } = expectedResultsHtml;

    /// <summary>
    /// Gets or sets whether the expected results should be appended to existing expected results.
    /// </summary>
    public bool Append { get; init; } = false;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TestResult target)
    {
        if (this.Append && target.ExpectedResultsHtml is { Length: > 0 })
        {
            target.ExpectedResultsHtml += this.ExpectedResultsHtml;
        }
        else
        {
            target.ExpectedResultsHtml = this.ExpectedResultsHtml;
        }
    }
}
