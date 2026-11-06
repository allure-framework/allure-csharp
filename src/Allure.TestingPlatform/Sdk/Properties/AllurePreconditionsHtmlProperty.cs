using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets or appends the preconditions of a test in the HTML format.
/// </summary>
/// <param name="preconditionsHtml">The HTML preconditions.</param>
public sealed class AllurePreconditionsHtmlProperty(string preconditionsHtml) :
    IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the preconditions HTML.
    /// </summary>
    public string PreconditionsHtml { get; } = preconditionsHtml;

    /// <summary>
    /// Gets or sets whether the preconditions should be appended to existing preconditions.
    /// </summary>
    public bool Append { get; init; } = false;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TestResult target)
    {
        if (this.Append && target.PreconditionsHtml is { Length: > 0 })
        {
            target.PreconditionsHtml += this.PreconditionsHtml;
        }
        else
        {
            target.PreconditionsHtml = this.PreconditionsHtml;
        }
    }
}
