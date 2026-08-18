using System.IO;
using Allure.Model;
using Allure.Sdk.Functions;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Attaches data to an Allure step, test, or fixture.
/// </summary>
/// <typeparam name="TModel">The type of model object to which the attachment is added.</typeparam>
/// <param name="name">The attachment name.</param>
/// <param name="content">The stream containing the attachment data.</param>
public sealed class AllureAttachmentProperty<TModel>(string name, Stream content) :
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
    public Stream Content { get; } = content;

    /// <summary>
    /// Gets or sets the attachment content type.
    /// </summary>
    public string? MediaType { get; init; }

    /// <summary>
    /// Gets or sets the attachment file extension.
    /// </summary>
    public string FileExtension { get; init; } = "";

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime, TModel target)
    {
        var source = AttachmentSource.CreateName(this.FileExtension);
        var attachment = new Attachment
        {
            Name = this.Name,
            Type = this.MediaType,
            Source = source,
            FileExtension = this.FileExtension,
        };
        allureRuntime.ResultsDestination.WriteAttachment(source, this.Content);
        target.Attachments.Add(attachment);
    }
}
