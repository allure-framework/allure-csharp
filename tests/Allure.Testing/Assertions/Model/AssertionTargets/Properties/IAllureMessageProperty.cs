namespace Allure.Testing.Assertions.Model.AssertionTargets.Properties;

public interface IAllureMessageProperty : IAllureStringProperty<IAllureMessageProperty>
{
    static string IAllureProperty<string, IAllureMessageProperty>.PropertyName { get; } =
        "message";
}
