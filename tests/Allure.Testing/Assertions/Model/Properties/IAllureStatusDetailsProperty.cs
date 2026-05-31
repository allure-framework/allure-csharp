namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureStatusDetailsProperty<TSelf> : IAllureObjectProperty<AllureStatusDetails, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureStatusDetailsProperty<TSelf>
{
    static string IAllureProperty<AllureStatusDetails, TSelf>.PropertyName { get; }
        = "statusDetails";
}
