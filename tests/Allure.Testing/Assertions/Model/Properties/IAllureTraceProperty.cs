namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureTraceProperty<TSelf> : IAllureStringProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureTraceProperty<TSelf>;
