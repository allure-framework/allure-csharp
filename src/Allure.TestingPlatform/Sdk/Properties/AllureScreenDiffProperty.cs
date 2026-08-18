using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Attaches a screen diff to an Allure step, test, or fixture.
/// </summary>
/// <typeparam name="TModel">The type of model object to which the screen diff is added.</typeparam>
/// <param name="expected">A stream containing the expected PNG image.</param>
/// <param name="actual">A stream containing the actual PNG image.</param>
/// <param name="diff">A stream containing the PNG image that visualizes the differences.</param>
public sealed class AllureScreenDiffProperty<TModel>(
    Stream expected,
    Stream actual,
    Stream diff
) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets or sets the attachment name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the expected image.
    /// </summary>
    public Stream Expected { get; } = expected;

    /// <summary>
    /// Gets the actual image.
    /// </summary>
    public Stream Actual { get; } = actual;

    /// <summary>
    /// Gets the difference image.
    /// </summary>
    public Stream Diff { get; } = diff;

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime, TModel target)
    {
        var name = this.Name ??
            string.Format(
                DIFF_NAME_PATTERN,
                target.Attachments.Count(
                    static (a) => a.Type == DIFF_MEDIA_TYPE
                ) + 1
            );

        using var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, new
        {
            expected = ToDiffEntry(this.Expected),
            actual = ToDiffEntry(this.Actual),
            diff = ToDiffEntry(this.Diff)
        });

        stream.Position = 0;

        new AllureAttachmentProperty<TModel>(name, stream)
        {
            MediaType = DIFF_MEDIA_TYPE,
            FileExtension = ".json",
        }.Apply(allureRuntime, target);
    }

    const int DefaultCopyBufferSize = 81920;
    const string DIFF_NAME_PATTERN = "diff-{0}";
    const string DIFF_MEDIA_TYPE = "application/vnd.allure.image.diff";
    const string DIFF_ENTRY_PREFIX = "data:image/png;base64,";

    static string ToDiffEntry(Stream data)
    {
        using var stream = new MemoryStream();
        data.CopyTo(stream, DefaultCopyBufferSize);
        var base64Part = Convert.ToBase64String(stream.ToArray());
        return $"{DIFF_ENTRY_PREFIX}{base64Part}";
    }
}
