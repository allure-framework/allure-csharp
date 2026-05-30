namespace Allure.Testing.Assertions.Model.Properties;

public interface IAllureMessageProperty<TSelf> : IAllureStringProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureMessageProperty<TSelf>
{
    static string IAllureProperty<string, TSelf>.PropertyName { get; } =
        "message";
}
