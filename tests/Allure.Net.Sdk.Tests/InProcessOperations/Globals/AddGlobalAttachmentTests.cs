using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Globals;

public class AddGlobalAttachmentTests
{
    [Test]
    public async Task AddGlobalAttachmentWritesContentAndGlobals()
    {
        var environment = AllureApiTestEnvironment.Create();
        using var content = new MemoryStream([1, 2, 3]);

        var before = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        environment.Run(_ => AllureApi.AddGlobalAttachment(
            "attachment",
            content,
            "application/octet-stream",
            ".bin"
        ));
        var after = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        var global = await Assert.That(environment.Destination.Globals).HasSingleItem();
        var attachment = await Assert.That(global.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("attachment");
        await Assert.That(attachment.Type)
            .IsEqualTo("application/octet-stream");
        await Assert.That(attachment.Source).EndsWith(".bin");
        await Assert.That(attachment.Timestamp).IsBetween(before, after);
        await Assert.That(
            environment.Destination.ByteAttachments[attachment.Source]
        ).IsEquivalentTo(new byte[] { 1, 2, 3 });
    }

    [Test]
    public async Task AddGlobalAttachmentAsyncWritesContentAndGlobals()
    {
        var environment = AllureApiTestEnvironment.Create();
        using var content = new MemoryStream([1, 2, 3]);

        var before = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await environment.RunAsync(_ => AllureApi.AddGlobalAttachmentAsync(
            "attachment",
            content,
            "application/octet-stream",
            ".bin",
            CancellationToken.None
        ));
        var after = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        var global = await Assert.That(environment.Destination.Globals).HasSingleItem();
        var attachment = await Assert.That(global.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("attachment");
        await Assert.That(attachment.Type)
            .IsEqualTo("application/octet-stream");
        await Assert.That(attachment.Source).EndsWith(".bin");
        await Assert.That(attachment.Timestamp).IsBetween(before, after);
        await Assert.That(
            environment.Destination.ByteAttachments[attachment.Source]
        ).IsEquivalentTo(new byte[] { 1, 2, 3 });
    }
}
