using System.Linq;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Links an already written screen diff attachment to an Allure step, test, or fixture.
/// </summary>
/// <typeparam name="TModel">The type of model object to which the attachment is added.</typeparam>
/// <param name="source">The name of the attachment file in the results directory.</param>
public sealed class AllureScreenDiffReferenceProperty<TModel>(string source) :
    IAllureProperty<TModel>
    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets or sets the attachment name.
    /// </summary>
    public string? Name { get; init; }

    /// <inheritdoc/>
    public void Apply(
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _,
        TModel target
    )
    {
        target.Attachments.Add(new()
        {
            Name = this.Name ?? NextScreenDiffName(target),
            Source = source,
            Type = ScreenDiffMediaType,
        });
    }

    const string ScreenDiffMediaType = "application/vnd.allure.image.diff";

    static string NextScreenDiffName(ExecutableItem target) =>
        $"Screen diff {target.Attachments.Count(
            static attachment => attachment.Type == ScreenDiffMediaType
        ) + 1}";
}
