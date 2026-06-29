using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureAttachmentProperty<TModel>(string name, byte[] content) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    public string Name { get; } = name;

    public byte[] Content { get; } = content;

    public string? ContentType { get; init; }

    public string FileExtension { get; init; } = "";

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