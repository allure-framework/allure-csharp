namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(PropertyName = "Type")]
public interface IAllureLinkTypeProperty<TSelf> : IAllureStringProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureLinkTypeProperty<TSelf>;
