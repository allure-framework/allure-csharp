namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureFlakyProperty<TSelf> : IAllureBoolProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureFlakyProperty<TSelf>
{
}
