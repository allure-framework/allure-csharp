namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(PropertyName = "Excluded")]
public interface IAllureParameterExcludedProperty<TSelf> : IAllureBoolProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureParameterExcludedProperty<TSelf>
{
}
