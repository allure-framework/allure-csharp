namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(ItemMethodName = "TearDownFixture")]
public interface IAllureAftersProperty<TSelf> : IAllureObjectArrayProperty<AllureFixtureResult, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureObjectArrayProperty<AllureFixtureResult, TSelf>;
