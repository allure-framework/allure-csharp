using TestResult = Allure.Model.TestResult;

namespace Allure.Net.Tests.Api.Attributes;

static class AttributeAssertions
{
    public static async Task AssertLabels(
        TestResult tr,
        params (string Name, string Value)[] expected
    )
    {
        await Assert.That(tr.Labels.Count).IsEqualTo(expected.Length);
        for (var index = 0; index < expected.Length; index++)
        {
            await Assert.That(tr.Labels[index].Name).IsEqualTo(expected[index].Name);
            await Assert.That(tr.Labels[index].Value).IsEqualTo(expected[index].Value);
        }
    }

    public static async Task AssertLink(
        TestResult tr,
        string url,
        string? name,
        string? type
    )
    {
        var link = await Assert.That(tr.Links).HasSingleItem();
        await Assert.That(link.Url).IsEqualTo(url);
        await Assert.That(link.Name).IsEqualTo(name);
        await Assert.That(link.Type).IsEqualTo(type);
    }
}
