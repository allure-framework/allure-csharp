using Allure.Abstractions;
using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

public class AllureMetaAttributeTests
{
    [Test]
    public async Task AttributesOfMetaAttributeAreApplied()
    {
        TestResult tr = new() { Name = "test", Uuid = "id" };

        new ProductMetadataAttribute().Apply(tr);

        await AttributeAssertions.AssertLabels(
            tr,
            ("tag", "smoke"),
            ("suite", "Checkout")
        );
        await AttributeAssertions.AssertLink(tr, "ISSUE-17", "Issue title", "issue");
    }

    [AllureTag("smoke")]
    [AllureSuite("Checkout")]
    [AllureIssue("ISSUE-17", Title = "Issue title")]
    [AttributeUsage(AllureApiAttribute.ALLURE_METADATA_TARGETS, AllowMultiple = true)]
    private sealed class ProductMetadataAttribute : AllureMetaAttribute;
}
