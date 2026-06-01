namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureKnownProperty<TSelf> : IAllureBoolProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureKnownProperty<TSelf>
{
}
