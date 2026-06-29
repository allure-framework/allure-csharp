using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureAttachmentFileProperty<TModel>(string name, string path) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    public string Name { get; } = name;

    public string Path { get; } = path;

    public string? ContentType { get; init; }

    public string FileExtension { get; init; } = System.IO.Path.GetExtension(path);

    public void Apply(LiveAllureTestingPlatformRuntime allureState, TModel target)
    {
        var source = ModelFunctions.GetAttachmentSourceName(this.FileExtension);
        var attachment = new Attachment
        {
            name = this.Name,
            type = this.ContentType,
            source = source
        };
        allureState.Writer.Write(source, this.Path);
        target.attachments.Add(attachment);
    }
}