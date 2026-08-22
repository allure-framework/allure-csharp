using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Attaches a file to an Allure step, test, or fixture.
/// </summary>
/// <typeparam name="TModel">The type of model object to which the attachment is added.</typeparam>
/// <param name="name">The attachment name.</param>
/// <param name="path">The path of the file to attach.</param>
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
    public string? MediaType { get; init; }

    /// <summary>
    /// Gets or sets the attachment file extension.
    /// </summary>
    public string FileExtension { get; init; } = System.IO.Path.GetExtension(path);

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime, TModel target)
    {
        var source = allureRuntime.ResultsDestination.CopyAttachment(
            this.Path,
            this.FileExtension
        );

        target.Attachments.Add(new Attachment
        {
            Name = this.Name,
            Type = this.MediaType,
            Source = source,
        });
    }
}
