namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(PropertyName = "MediaType", JsonName = "type")]
public interface IAllureMediaTypeProperty<TSelf> : IAllureStringProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureMediaTypeProperty<TSelf>;
