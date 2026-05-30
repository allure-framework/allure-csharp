namespace Allure.Testing.Assertions.Model.Properties;

public interface IAllureNameProperty<TSelf> : IAllureStringProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureNameProperty<TSelf>
{
    static string IAllureProperty<string, TSelf>.PropertyName { get; } =
        "name";
}
