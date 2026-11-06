namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(JsonName = "precondition")]
public interface IAllurePreconditionsProperty<TSelf> : IAllureStringProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllurePreconditionsProperty<TSelf>;
