namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureStartProperty<TSelf> : IAllureLongProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureStartProperty<TSelf>;
