using Allure.Net.Commons;
using Allure.Net.Commons.Functions;

namespace Allure.TestingPlatform.Properties;

public sealed class AllureAttachmentProperty<TObject>(string name, byte[] content) : IAllureProperty<TObject>
    where TObject : ExecutableItem
{
    public string Name { get; } = name;

    public byte[] Content { get; } = content;

    public string? ContentType { get; init; }

    public string FileExtension { get; init; } = "";

    public void Apply(IAllureInfrastructure allure, TObject obj)
    {
        var source = ModelFunctions.GetAttachmentSourceName(this.FileExtension);
        var attachment = new Attachment
        {
            name = this.Name,
            type = this.ContentType,
            source = source
        };
        allure.Writer.Write(source, this.Content);
        allure.Lifecycle.UpdateExecutableItem((item) => item.attachments.Add(attachment));
    }
}