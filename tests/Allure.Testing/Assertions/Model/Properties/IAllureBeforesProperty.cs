namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureBeforesProperty<TSelf> : IAllureObjectArrayProperty<AllureFixtureResult, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureObjectArrayProperty<AllureFixtureResult, TSelf>
{

}
