namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(JsonName = "expectedResultHtml")]
public interface IAllureExpectedResultsHtmlProperty<TSelf> : IAllureStringProperty<TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureExpectedResultsHtmlProperty<TSelf>;
