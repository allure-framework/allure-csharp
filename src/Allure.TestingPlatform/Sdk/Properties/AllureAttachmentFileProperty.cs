using Allure.Net.Commons;
using Allure.Net.Commons.Functions;

namespace Allure.TestingPlatform.Sdk.Properties;

public sealed class AllureAttachmentFileProperty<TObject>(string name, string path) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string Name { get; } = name;

    public string Path { get; } = path;

    public string? ContentType { get; init; }

    public string FileExtension { get; init; } = System.IO.Path.GetExtension(path);

    public void Apply(IAllureRuntime allure, TObject obj)
    {
        var source = ModelFunctions.GetAttachmentSourceName(this.FileExtension);
        var attachment = new Attachment
        {
            name = this.Name,
            type = this.ContentType,
            source = source
        };
        allure.Writer.Write(source, this.Path);
        allure.Lifecycle.UpdateExecutableItem((item) => item.attachments.Add(attachment));
    }
}