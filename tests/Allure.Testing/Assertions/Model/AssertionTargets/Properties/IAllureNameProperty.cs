namespace Allure.Testing.Assertions.Model.AssertionTargets.Properties;

public interface IAllureNameProperty : IAllureStringProperty<IAllureNameProperty>
{
    static string IAllureProperty<string, IAllureNameProperty>.PropertyName { get; } =
        "name";
}
