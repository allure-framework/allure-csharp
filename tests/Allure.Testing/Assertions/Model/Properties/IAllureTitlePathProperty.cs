namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureTitlePathProperty<TSelf> : IAllureArrayProperty<string, IStringItemFactory, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureTitlePathProperty<TSelf>
{
}
