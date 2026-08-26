using System.IO;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Links an already written attachment to an Allure step, test, or fixture.
/// </summary>
/// <typeparam name="TModel">The type of model object to which the attachment is added.</typeparam>
/// <param name="name">The attachment name.</param>
/// <param name="source">The name of the attachment file in the results directory.</param>
/// <param name="mediaType">The media type of the attachment.</param>
public sealed class AllureAttachmentReferenceProperty<TModel>(
    string name,
    string source,
    string? mediaType
) : IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    /// <inheritdoc/>
    public void Apply(
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _,
        TModel target
    )
    {
        target.Attachments.Add(new()
        {
            Name = name,
            Source = source,
            Type = mediaType,
        });
    }
}
