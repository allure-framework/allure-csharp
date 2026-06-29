using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureAttachmentFileProperty<TObject>(string name, string path) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string Name { get; } = name;

    public string Path { get; } = path;

    public string? ContentType { get; init; }

    public string FileExtension { get; init; } = System.IO.Path.GetExtension(path);

    public void Apply(ReadyAllureTestingPlatformRuntime allureState, TObject obj)
    {
        var source = ModelFunctions.GetAttachmentSourceName(this.FileExtension);
        var attachment = new Attachment
        {
            name = this.Name,
            type = this.ContentType,
            source = source
        };
        allureState.Writer.Write(source, this.Path);
        obj.attachments.Add(attachment);
    }
}