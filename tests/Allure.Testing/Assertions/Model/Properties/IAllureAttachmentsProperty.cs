namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureAttachmentsProperty<TSelf> : IAllureObjectArrayProperty<AllureAttachment, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureAttachmentsProperty<TSelf>;
