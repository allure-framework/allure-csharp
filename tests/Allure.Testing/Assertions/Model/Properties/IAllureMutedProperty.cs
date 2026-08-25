namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureMutedProperty<TSelf> : IAllureBoolProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureMutedProperty<TSelf>;
