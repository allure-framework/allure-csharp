using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Attaches data to an Allure step, test, or fixture.
/// </summary>
public sealed class AllureAttachmentProperty<TModel>(string name, byte[] content) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the attachment name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the attachment content.
    /// </summary>
    public byte[] Content { get; } = content;

    /// <summary>
    /// Gets or sets the attachment content type.
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// Gets or sets the attachment file extension.
    /// </summary>
    public string FileExtension { get; init; } = "";

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
        allureRuntime.Writer.Write(source, this.Content);
        target.attachments.Add(attachment);
    }
}
