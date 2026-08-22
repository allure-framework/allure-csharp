using Allure.Model;
using Allure.Sdk.Results;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.Results;

public class NullResultsDestinationTests
{
    [Test]
    public async Task ShouldIgnoreAllSynchronousWrites()
    {
        var destination = NullResultsDestination.Instance;

        destination.WriteTestResult(NewTestResult());
        destination.WriteContainer(NewContainer());
        destination.WriteGlobals(new Globals());
        destination.WriteAttachment(Stream.Null, ".bin");
        destination.CopyAttachment("does-not-need-to-exist.bin", ".bin");

        await Assert.That(destination).IsSameReferenceAs(NullResultsDestination.Instance);
    }

    [Test]
    public async Task ShouldIgnoreAllAsynchronousWritesIncludingCancellation()
    {
        var destination = NullResultsDestination.Instance;
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await destination.WriteTestResultAsync(NewTestResult(), cancellation.Token);
        await destination.WriteContainerAsync(NewContainer(), cancellation.Token);
        await destination.WriteGlobalsAsync(new Globals(), cancellation.Token);
        await destination.WriteAttachmentAsync(Stream.Null, ".bin", cancellation.Token);
        await destination.CopyAttachmentAsync(
            "does-not-need-to-exist.bin",
            ".bin",
            cancellation.Token
        );

        await Assert.That(cancellation.IsCancellationRequested).IsTrue();
    }

    static AllureTestResult NewTestResult() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "test",
    };

    static TestResultScope NewContainer() => new()
    {
        Uuid = Guid.NewGuid().ToString(),
        Name = "container",
    };
}
