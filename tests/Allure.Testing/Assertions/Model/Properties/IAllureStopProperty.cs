namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureStopProperty<TSelf> : IAllureLongProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureStopProperty<TSelf>;
