namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(PropertyName = "Source")]
public interface IAllureAttachmentSourceProperty<TSelf> : IAllureStringProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureAttachmentSourceProperty<TSelf>
{
}
