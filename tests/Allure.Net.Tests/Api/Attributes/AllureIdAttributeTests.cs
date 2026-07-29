using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureIdAttributeTests
{
    [Test]
    public async Task AllureIdCanBeAddedToTest()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };
        var attr = new AllureIdAttribute(1001);

        attr.Apply(tr);

        await Assert.That(attr.Name).IsEqualTo("ALLURE_ID");
        await Assert.That(attr.Value).IsEqualTo("1001");
        await AttributeAssertions.AssertLabels(tr, ("ALLURE_ID", "1001"));
    }
}
