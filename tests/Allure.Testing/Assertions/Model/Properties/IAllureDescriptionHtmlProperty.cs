namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureDescriptionHtmlProperty<TSelf> : IAllureStringProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureDescriptionHtmlProperty<TSelf>;
