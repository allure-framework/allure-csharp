namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureTimestampProperty<TSelf> : IAllureLongProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureTimestampProperty<TSelf>;
