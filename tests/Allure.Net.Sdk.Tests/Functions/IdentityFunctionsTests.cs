using Allure.Model;
using Allure.Sdk.Functions;

namespace Allure.Net.Sdk.Tests.Functions;

public class IdentityFunctionsTests
{
    [Test]
    public async Task ShouldCalculateMd5Hashes()
    {
        await Assert.That(Md5.FromString("hello")).IsEqualTo("5d41402abc4b2a76b9719d911017c592");
        await Assert.That(Md5.FromJson(new { Name = "value", Count = 2 }))
            .IsEqualTo(Md5.FromString("{\"Name\":\"value\",\"Count\":2}"));
    }

    [Test]
    public async Task ShouldCreateStableTestCaseAndHistoryIds()
    {
        var parameters = new[]
        {
            new Parameter { Name = "b", Value = "two" },
            new Parameter { Name = "a", Value = "one" },
        };

        var historyId = Ids.ForTest("sample", parameters);
        var reordered = Ids.ForTest("sample", parameters.Reverse());
        var changed = Ids.ForTest("sample", [
            new Parameter { Name = "a", Value = "different" },
            new Parameter { Name = "b", Value = "two" },
        ]);

        await Assert.That(Ids.ForTestCase("sample")).IsEqualTo(Md5.FromString("sample"));
        await Assert.That(historyId).IsEqualTo(reordered);
        await Assert.That(historyId).IsNotEqualTo(changed);
        await Assert.That(Guid.TryParse(Ids.NewUuid(), out _)).IsTrue();
    }

    [Test]
    public async Task HistoryIdsDoNotDependOnExcludedParameters()
    {
        var p1 = new Parameter { Name = "b", Value = "two" };
        var p2 = new Parameter { Name = "a", Value = "one" };
        var excluded = new Parameter { Name = "ignored", Value = "random", Excluded = true };

        List<Parameter> withoutExcluded = [ p1, p2 ];
        List<Parameter> withExcluded = [ .. withoutExcluded, excluded ];

        await Assert.That(Ids.ForTest("sample", withoutExcluded)).IsEqualTo(Ids.ForTest("sample", withExcluded));
    }
}
