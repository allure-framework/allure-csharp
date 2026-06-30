using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Attaches a file to an Allure step, test, or fixture.
/// </summary>
public sealed class AllureAttachmentFileProperty<TModel>(string name, string path) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the attachment name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the source file path.
    /// </summary>
    public string Path { get; } = path;

    /// <summary>
    /// Gets or sets the attachment content type.
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// Gets or sets the attachment file extension.
    /// </summary>
    public string FileExtension { get; init; } = System.IO.Path.GetExtension(path);

    /// <inheritdoc />
    public void Apply(LiveAllureTestingPlatformRuntime allureRuntime, TModel target)
    {
        var source = ModelFunctions.GetAttachmentSourceName(this.FileExtension);
        var attachment = new Attachment
        {
            name = this.Name,
            type = this.ContentType,
            source = source
        };
        allureRuntime.Writer.Write(source, this.Path);
        target.attachments.Add(attachment);
    }
}
