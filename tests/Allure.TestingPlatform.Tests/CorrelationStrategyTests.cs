using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Tests;

public class CorrelationStrategyTests
{
    [Test]
    public async Task ShouldCorrelateByTestingPlatformSessionUid()
    {
        SessionUidCorrelationStrategy strategy = new();
        TestNodeUpdateMessage message = new(
            new("session-1"),
            new()
            {
                DisplayName = "test",
                Uid = "test-1"
            }
        );

        var correlation = await strategy.GetCorrelationAsync(
            DataProducerStub.Instance,
            message,
            CancellationToken.None
        );

        await Assert.That(correlation).IsEqualTo(new CorrelationUid("session-1"));
    }

    [Test]
    public async Task ShouldReadCorrelationFromTestNodeMetadata()
    {
        TestNodeMetadataCorrelationStrategy strategy = new();
        TestNodeUpdateMessage message = new(
            new("session-1"),
            new()
            {
                DisplayName = "test",
                Uid = "test-1",
                Properties = new(
                    new TestMetadataProperty(
                        TestNodeMetadataCorrelationStrategy.MetadataKey,
                        "correlation-1"
                    )
                )
            }
        );

        var correlation = await strategy.GetCorrelationAsync(
            DataProducerStub.Instance,
            message,
            CancellationToken.None
        );

        await Assert.That(correlation).IsEqualTo(new CorrelationUid("correlation-1"));
    }

    [Test]
    public async Task ShouldReturnNullWhenTestNodeHasNoCorrelationMetadata()
    {
        TestNodeMetadataCorrelationStrategy strategy = new();
        TestNodeUpdateMessage message = new(
            new("session-1"),
            new()
            {
                DisplayName = "test",
                Uid = "test-1",
                Properties = new(new TestMetadataProperty("other", "ignored"))
            }
        );

        var correlation = await strategy.GetCorrelationAsync(
            DataProducerStub.Instance,
            message,
            CancellationToken.None
        );

        await Assert.That(correlation).IsNull();
    }

    [Test]
    public async Task ShouldReturnNullWhenMessageIsNotTestNodeUpdate()
    {
        TestNodeMetadataCorrelationStrategy strategy = new();
        SessionFileArtifact message = new(new("session-1"), new("artifact.txt"), "artifact");

        var correlation = await strategy.GetCorrelationAsync(
            DataProducerStub.Instance,
            message,
            CancellationToken.None
        );

        await Assert.That(correlation).IsNull();
    }

    [Test]
    public async Task ShouldCreateGuidCorrelationUid()
    {
        var correlationUid = TestNodeMetadataCorrelationStrategy.CreateCorrelationUid();

        await Assert.That(Guid.TryParse(correlationUid, out _)).IsTrue();
    }
}
