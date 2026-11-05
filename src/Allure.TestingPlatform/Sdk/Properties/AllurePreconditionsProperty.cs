using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Sets or appends the Markdown preconditions of a test.
/// </summary>
/// <param name="preconditions">The Markdown preconditions.</param>
public sealed class AllurePreconditionsProperty(string preconditions) :
    IAllureProperty<TestResult>
{
    /// <summary>
    /// Gets the preconditions Markdown text.
    /// </summary>
    public string Preconditions { get; } = preconditions;

    /// <summary>
    /// Gets or sets whether the preconditions should be appended to existing preconditions.
    /// </summary>
    public bool Append { get; init; } = false;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TestResult target)
    {
        if (this.Append && target.Preconditions is { Length: > 0 })
        {
            target.Preconditions += $"\n\n{this.Preconditions}";
        }
        else
        {
            target.Preconditions = this.Preconditions;
        }
    }
}
