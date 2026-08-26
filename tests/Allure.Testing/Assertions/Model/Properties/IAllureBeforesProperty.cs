namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(ItemMethodName = "SetUpFixture")]
public interface IAllureBeforesProperty<TSelf> : IAllureObjectArrayProperty<AllureFixtureResult, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureObjectArrayProperty<AllureFixtureResult, TSelf>;
