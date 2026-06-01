namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureParameterExcludedProperty<TSelf> : IAllureBoolProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureParameterExcludedProperty<TSelf>
{
}
