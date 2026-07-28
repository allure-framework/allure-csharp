using Allure.Net.Sdk.Tests.Infrastructure;
using Mono.Cecil.Cil;

namespace Allure.Net.Sdk.Tests.InProcessOperations.Globals;

public class AddGlobalAttachmentFromFileTests
{
    [Test]
    public async Task AddGlobalAttachmentFromFileCopiesFileAndWritesGlobals()
    {
        var environment = AllureApiTestEnvironment.Create();
        const string path = "/input/report.json";

        var before = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        environment.Run(_ => AllureApi.AddGlobalAttachmentFromFile(
            path,
            "report",
            "application/json",
            ".json"
        ));
        var after = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        var global = await Assert.That(environment.Destination.Globals).HasSingleItem();
        var attachment = await Assert.That(global.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("report");
        await Assert.That(attachment.Type).IsEqualTo("application/json");
        await Assert.That(attachment.Source).EndsWith(".json");
        await Assert.That(attachment.Timestamp).IsBetween(before, after);
        await Assert.That(
            environment.Destination.FileAttachments[attachment.Source]
        ).IsEqualTo(path);
    }

    [Test]
    public async Task AddGlobalAttachmentFromFileAsyncCopiesFileAndWritesGlobals()
    {
        var environment = AllureApiTestEnvironment.Create();
        const string path = "/input/report.json";

        var before = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        await environment.RunAsync(
            _ => AllureApi.AddGlobalAttachmentFromFileAsync(
                path,
                "report",
                "application/json",
                ".json",
                CancellationToken.None
            )
        );
        var after = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        var global = await Assert.That(environment.Destination.Globals).HasSingleItem();
        var attachment = await Assert.That(global.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("report");
        await Assert.That(attachment.Type).IsEqualTo("application/json");
        await Assert.That(attachment.Source).EndsWith(".json");
        await Assert.That(attachment.Timestamp).IsBetween(before, after);
        await Assert.That(
            environment.Destination.FileAttachments[attachment.Source]
        ).IsEqualTo(path);
    }
}
