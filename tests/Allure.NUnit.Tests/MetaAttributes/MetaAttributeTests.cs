using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests.MetaAttributes;

class MetaAttributeTests
{
    [Test]
    public async Task MetaAttributeShouldWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.MetaAttributes);

        await Assert.That(results.TestResults.Cast<JsonObject>()).Count().IsEqualTo(1);

        var labels = results.TestResults[0]["labels"].AsArray().Cast<JsonObject>();
        var links = results.TestResults[0]["links"].AsArray().Cast<JsonObject>();

        await Assert.That(labels).Any(
            static (l) =>
            {
                return (string)l["name"] == "epic" && (string)l["value"] == "Foo";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "owner" && (string)l["value"] == "John Doe";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "feature" && (string)l["value"] == "Bar";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "tag" && (string)l["value"] == "foo";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "tag" && (string)l["value"] == "bar";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "story" && (string)l["value"] == "Baz";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "severity" && (string)l["value"] == "critical";
            }
        ).And.Any(
            static (l) =>
            {
                return (string)l["name"] == "suite" && (string)l["value"] == "Qux";
            }
        );
        await Assert.That(links).Any(
            static (l) =>
                (string)l["url"] == "https://foo.bar/"
                    && l["name"] is null
                    && l["type"] is null
        );
    }
}
