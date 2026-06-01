namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(ItemMethodName = "AfterFixture")]
public interface IAllureAftersProperty<TSelf> : IAllureObjectArrayProperty<AllureFixtureResult, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureObjectArrayProperty<AllureFixtureResult, TSelf>
{
}
