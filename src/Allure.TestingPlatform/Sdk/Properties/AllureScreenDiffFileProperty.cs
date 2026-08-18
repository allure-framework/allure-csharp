using System.IO;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Attaches a screen diff to an Allure step, test, or fixture.
/// </summary>
/// <typeparam name="TModel">The type of model object to which the screen diff is added.</typeparam>
/// <param name="expectedPath">The path of the expected PNG image.</param>
/// <param name="actualPath">The path of the actual PNG image.</param>
/// <param name="diffPath">The path of the PNG image that visualizes the differences.</param>
public sealed class AllureScreenDiffFileProperty<TModel>(
    string expectedPath,
    string actualPath,
    string diffPath
) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets or sets the attachment name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the expected image path.
    /// </summary>
    public string ExpectedPath { get; } = expectedPath;

    /// <summary>
    /// Gets the actual image path.
    /// </summary>
    public string ActualPath { get; } = actualPath;

    /// <summary>
    /// Gets the difference image path.
    /// </summary>
    public string DiffPath { get; } = diffPath;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime, TModel target)
    {
        using var expected = File.OpenRead(this.ExpectedPath);
        using var actual = File.OpenRead(this.ActualPath);
        using var diff = File.OpenRead(this.DiffPath);

        new AllureScreenDiffProperty<TModel>(expected, actual, diff)
        {
            Name = this.Name,
        }.Apply(allureRuntime, target);
    }
}
