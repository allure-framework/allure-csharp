namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(JsonName = "preconditionHtml")]
public interface IAllurePreconditionsHtmlProperty<TSelf> : IAllureStringProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllurePreconditionsHtmlProperty<TSelf>;
