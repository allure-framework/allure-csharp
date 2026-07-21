using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class ScreenDiffTests
{
    [Test]
    public async Task SyncStreamScreenDiffPreservesOrderingAndContent()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(current: recorder.Endpoint);
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);

        AllureApi.AddScreenDiff(expected, actual, diff);

        await AssertScreenDiff(recorder.ScreenDiffs.Single(), [1], [2], [3]);
    }

    [Test]
    public async Task SyncMemoryScreenDiffPreservesOrderingAndContent()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(current: recorder.Endpoint);

        AllureApi.AddScreenDiff(
            new ReadOnlyMemory<byte>([1]),
            new ReadOnlyMemory<byte>([2]),
            new ReadOnlyMemory<byte>([3])
        );

        await AssertScreenDiff(recorder.ScreenDiffs.Single(), [1], [2], [3]);
    }

    [Test]
    public async Task AsyncStreamAndMemoryScreenDiffsPreserveToken()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(current: recorder.Endpoint);
        using var cancellation = new CancellationTokenSource();
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);

        await AllureApi.AddScreenDiffAsync(expected, actual, diff, cancellation.Token);
        await AllureApi.AddScreenDiffAsync(
            new ReadOnlyMemory<byte>([4]),
            new ReadOnlyMemory<byte>([5]),
            new ReadOnlyMemory<byte>([6]),
            cancellation.Token
        );

        await AssertScreenDiff(recorder.ScreenDiffs[0], [1], [2], [3]);
        await AssertScreenDiff(recorder.ScreenDiffs[1], [4], [5], [6]);
        await Assert.That(recorder.ScreenDiffs.All(item =>
            item.CancellationToken == cancellation.Token
        )).IsTrue();
    }

    [Test]
    public async Task FileScreenDiffPreservesPathOrderingForSyncAndAsync()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(current: recorder.Endpoint);
        using var cancellation = new CancellationTokenSource();

        AllureApi.AddFileScreenDiff("expected.png", "actual.png", "diff.png");
        await AllureApi.AddFileScreenDiffAsync(
            "expected-async.png",
            "actual-async.png",
            "diff-async.png",
            cancellation.Token
        );

        await Assert.That(recorder.ScreenDiffs[0].ExpectedPath).IsEqualTo("expected.png");
        await Assert.That(recorder.ScreenDiffs[0].ActualPath).IsEqualTo("actual.png");
        await Assert.That(recorder.ScreenDiffs[0].DiffPath).IsEqualTo("diff.png");
        await Assert.That(recorder.ScreenDiffs[1].ExpectedPath).IsEqualTo("expected-async.png");
        await Assert.That(recorder.ScreenDiffs[1].ActualPath).IsEqualTo("actual-async.png");
        await Assert.That(recorder.ScreenDiffs[1].DiffPath).IsEqualTo("diff-async.png");
        await Assert.That(recorder.ScreenDiffs[1].CancellationToken).IsEqualTo(cancellation.Token);
    }

    static async Task AssertScreenDiff(
        CapturedScreenDiff screenDiff,
        byte[] expected,
        byte[] actual,
        byte[] diff
    )
    {
        await Assert.That(screenDiff.Expected).IsEquivalentTo(expected);
        await Assert.That(screenDiff.Actual).IsEquivalentTo(actual);
        await Assert.That(screenDiff.Diff).IsEquivalentTo(diff);
    }
}
