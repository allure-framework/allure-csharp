namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(JsonName = "expectedResult")]
public interface IAllureExpectedResultsProperty<TSelf> : IAllureStringProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureExpectedResultsProperty<TSelf>;
