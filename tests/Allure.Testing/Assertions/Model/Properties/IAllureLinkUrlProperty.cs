namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(PropertyName = "Url")]
public interface IAllureLinkUrlProperty<TSelf> : IAllureStringProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureLinkUrlProperty<TSelf>
{
}
