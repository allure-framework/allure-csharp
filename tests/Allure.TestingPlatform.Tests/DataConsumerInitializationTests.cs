using System.Xml.Linq;
using Allure.TestingPlatform.Sdk.Messages;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Tests;

public class DataConsumerInitializationTests : DataConsumerTestsBase
{
    [Test]
    public async Task ShouldBeEnabled()
    {
        await Assert.That(this.consumer.IsEnabledAsync).IsTrue();
    }

    [Test]
    public async Task ShouldHaveLifecycle()
    {
        await Assert.That(this.consumer.Lifecycle).IsSameReferenceAs(this.lifecycle);
    }

    [Test]
    public async Task ShouldHaveConstantUid()
    {
        await Assert.That(this.consumer.Uid).IsEqualTo("dd4f3277-5786-4010-8908-e70f07656ebc");
    }

    [Test]
    public async Task ShouldHaveNameAndDescription()
    {
        await Assert.That(this.consumer)
            .Member(c => c.DisplayName, v => v.IsEqualTo("Allure.TestingPlatform data consumer"))
            .And.Member(
                c => c.Description,
                v => v.IsEqualTo(
                    "Creates Allure results from Microsoft Testing Platform messages"));
    }

    [Test]
    public async Task ShouldHaveVersionFromRootDirectoryProps()
    {

        using var stream = File.OpenRead("Directory.Build.props");
        var props = await XDocument.LoadAsync(stream, default, CancellationToken.None);
        var version = props.Root
            .Elements("PropertyGroup")
            .Select(e => e.Element("Version"))
            .First(e => e is not null)
            .Value;

        await Assert.That(this.consumer.Version).IsEqualTo(version);
    }

    [Test]
    public async Task ShouldSubscribeToNodeUpdatesAndAttachments()
    {
        await Assert.That(this.consumer.DataTypesConsumed).IsEquivalentTo([
            typeof(TestNodeUpdateMessage),
            typeof(SessionFileArtifact),

            typeof(AllureScopeStartMessage),
            typeof(AllureScopeStopMessage),

            typeof(AllureBeforeFixtureStartMessage),
            typeof(AllureAfterFixtureStartMessage),
            typeof(AllureFixtureUpdateMessage),
            typeof(AllureFixtureStopMessage),

            typeof(AllureTestsScopeMessage),

            typeof(AllureTestUpdateMessage),
        ]);
    }
}
