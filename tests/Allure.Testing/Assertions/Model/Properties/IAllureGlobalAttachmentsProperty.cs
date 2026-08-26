namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(
    PropertyName = "Attachments",
    MethodName = "GlobalAttachments",
    ItemMethodName = "GlobalAttachment")]
public interface IAllureGlobalAttachmentsProperty<TSelf> : IAllureObjectArrayProperty<AllureGlobalAttachment, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureGlobalAttachmentsProperty<TSelf>;
