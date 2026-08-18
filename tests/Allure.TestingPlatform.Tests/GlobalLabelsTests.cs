using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;
using Allure.TestingPlatform.Tests.Stubs;
using System.Collections.Immutable;
using Allure.TestingPlatform.Configuration;

namespace Allure.TestingPlatform.Tests;

public class GlobalLabelsTests : DataConsumerTestsBase
{
    protected override AllureTestingPlatformConfiguration Config => new()
    {
        GlobalLabels = ImmutableDictionary.Create<string, string>()
            .Add("globalLabel", "globalValue"),
    };

    [Test]
    public async Task ShouldAddGlobalLabelsFromConfigurationWhenCreatingTestResult()
    {
        var testNode = new TestNode
        {
            DisplayName = "Foo",
            Uid = "1",
            Properties = new(
                new PassedTestNodeStateProperty()
            )
        };

        var message = new TestNodeUpdateMessage(new SessionUid("Bar"), testNode);

        await this.consumer.ConsumeAsync(DataProducerStub.Instance, message, CancellationToken.None);

        var testResult = await Assert.That(this.writer.TestResults).HasSingleItem();
        await Assert.That(testResult.Labels).Contains(
            l => l.Name == "globalLabel" && l.Value == "globalValue"
        );
    }
}
