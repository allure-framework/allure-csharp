using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets or appends the Markdown expected results of a test.
/// </summary>
/// <param name="expectedResults">The Markdown expected results.</param>
public sealed class AllureExpectedResultsProperty(string expectedResults) :
    IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the expected results Markdown text.
    /// </summary>
    public string ExpectedResults { get; } = expectedResults;

    /// <summary>
    /// Gets or sets whether the expected results should be appended to existing expected results.
    /// </summary>
    public bool Append { get; init; } = false;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TestResult target)
    {
        if (this.Append && target.ExpectedResults is { Length: > 0 })
        {
            target.ExpectedResults += $"\n\n{this.ExpectedResults}";
        }
        else
        {
            target.ExpectedResults = this.ExpectedResults;
        }
    }
}
