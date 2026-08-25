namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(PropertyName = "Errors")]
public interface IAllureGlobalErrorsProperty<TSelf> : IAllureObjectArrayProperty<AllureGlobalError, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureGlobalErrorsProperty<TSelf>;
