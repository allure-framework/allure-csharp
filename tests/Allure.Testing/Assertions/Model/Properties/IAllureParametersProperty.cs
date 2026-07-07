namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureParametersProperty<TSelf> : IAllureObjectArrayProperty<AllureParameter, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureParametersProperty<TSelf>;
