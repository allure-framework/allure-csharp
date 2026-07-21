using System.Text;
using Allure.Model;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class GlobalAttachmentOverloadTests
{
    [Test]
    public async Task GlobalContentFamiliesUseOnlyGlobalEndpoint()
    {
        var current = new AttachmentRecorder();
        var global = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(current.Endpoint, global.Endpoint);
        using var stream = new MemoryStream([1, 2]);

        AllureApi.AddGlobalAttachment("stream", stream, "type/stream", ".stream");
        AllureApi.AddGlobalAttachment("memory", new ReadOnlyMemory<byte>([3, 4]), "type/memory", ".memory");

        await Assert.That(current.Attachments).IsEmpty();
        await Assert.That(global.Attachments.Count).IsEqualTo(2);
        await Assert.That(global.Attachments[0].Content).IsEquivalentTo(new byte[] { 1, 2 });
        await Assert.That(global.Attachments[1].Content).IsEquivalentTo(new byte[] { 3, 4 });
        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(0);
        await Assert.That(scope.GlobalResolutionCount).IsEqualTo(2);
    }

    [Test]
    public async Task AsyncGlobalContentFamiliesPreserveBytesAndToken()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(global: recorder.Endpoint);
        using var cancellation = new CancellationTokenSource();

        await AllureApi.AddGlobalAttachmentAsync("memory", new ReadOnlyMemory<byte>([1]), cancellation.Token);
        await AllureApi.AddGlobalAttachmentAsync("text", "Hello", "text/plain", ".txt", cancellation.Token);

        await Assert.That(recorder.Attachments[0].Content).IsEquivalentTo(new byte[] { 1 });
        await Assert.That(recorder.Attachments[0].MediaType).IsNull();
        await Assert.That(recorder.Attachments[1].Content)
            .IsEquivalentTo(Encoding.UTF8.GetBytes("Hello"));
        await Assert.That(recorder.Attachments[1].MediaType).IsEqualTo("text/plain");
        await Assert.That(recorder.Attachments.All(item => item.CancellationToken == cancellation.Token)).IsTrue();
    }

    [Test]
    public async Task GlobalFileAttachmentDerivesNameAndExtension()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(global: recorder.Endpoint);
        var path = Path.Combine("directory", "artifact.json");

        AllureApi.AddGlobalFileAttachment(path);

        var attachment = recorder.Attachments.Single();
        await Assert.That(attachment.Name).IsEqualTo("artifact.json");
        await Assert.That(attachment.Path).IsEqualTo(path);
        await Assert.That(attachment.MediaType).IsNull();
        await Assert.That(attachment.FileExtension).IsEqualTo(".json");
    }

    [Test]
    public async Task AsyncGlobalFileAttachmentPreservesExplicitArgumentsAndToken()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(global: recorder.Endpoint);
        using var cancellation = new CancellationTokenSource();

        await AllureApi.AddGlobalFileAttachmentAsync(
            "artifact.bin",
            "display",
            "application/example",
            ".custom",
            cancellation.Token
        );

        var attachment = recorder.Attachments.Single();
        await Assert.That(attachment.Name).IsEqualTo("display");
        await Assert.That(attachment.MediaType).IsEqualTo("application/example");
        await Assert.That(attachment.FileExtension).IsEqualTo(".custom");
        await Assert.That(attachment.CancellationToken).IsEqualTo(cancellation.Token);
    }

    [Test]
    public async Task GlobalErrorOverloadsPreserveDetailsAndPopulateTimestamp()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(global: recorder.Endpoint);
        var before = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var exception = new InvalidOperationException("exception message");
        var details = new StatusDetails
        {
            Message = "details message",
            Trace = "details trace",
            Flaky = true,
            Known = true,
            Muted = true,
        };

        AllureApi.AddGlobalError(exception);
        AllureApi.AddGlobalError("string message");
        AllureApi.AddGlobalError(details);
        var after = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        await Assert.That(recorder.GlobalErrors.All(error =>
            error.Timestamp >= before && error.Timestamp <= after
        )).IsTrue();
        await Assert.That(recorder.GlobalErrors[0].Message).IsEqualTo("exception message");
        await Assert.That(recorder.GlobalErrors[0].Trace).Contains(nameof(InvalidOperationException));
        await Assert.That(recorder.GlobalErrors[1].Message).IsEqualTo("string message");
        await Assert.That(recorder.GlobalErrors[2].Message).IsEqualTo("details message");
        await Assert.That(recorder.GlobalErrors[2].Trace).IsEqualTo("details trace");
        await Assert.That(recorder.GlobalErrors[2].Flaky).IsTrue();
        await Assert.That(recorder.GlobalErrors[2].Known).IsTrue();
        await Assert.That(recorder.GlobalErrors[2].Muted).IsTrue();
    }

    [Test]
    public async Task AsyncGlobalErrorPreservesTokenAndDetails()
    {
        var recorder = new AttachmentRecorder();
        using var scope = FacadeTestEnvironment.Use(global: recorder.Endpoint);
        using var cancellation = new CancellationTokenSource();

        await AllureApi.AddGlobalErrorAsync("failure", cancellation.Token);

        await Assert.That(recorder.GlobalErrors.Single().Message).IsEqualTo("failure");
        await Assert.That(recorder.Async.SingleCall.Arguments[^1]).IsEqualTo(cancellation.Token);
    }
}
