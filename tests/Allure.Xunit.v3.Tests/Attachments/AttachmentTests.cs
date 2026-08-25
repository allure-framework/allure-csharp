using System.Text.Json;
using Allure.Testing;
using Allure.Testing.Assertions.Model;
using TUnit.Assertions.Enums;

namespace Allure.Xunit.v3.Tests.Attachments;

class AttachmentTests
{
    static readonly AsyncLocal<AllureResults> results = new();

    [Before(Class)]
    public static async Task BeforeAll(ClassHookContext context, CancellationToken token)
    {
        results.Value = await AllureSampleRunner.RunAsync(
            AllureSampleRegistry.AttachmentApi,
            token
        );
        context.AddAsyncLocalValues();
    }

    [Test]
    public async Task SyncRuntimeAttachmentsWork()
    {
        var testResult = await Assert.That(results.Value)
            .HasSingleTestResult(tr => tr.HasName().That.EndsWith(".SyncRuntimeAttachments.TestMethod"))
            .With.Status(AllureStatus.Passed);

        await Assert.That(testResult).HasAttachmentsMatching([
            (a) => a.HasName("Sync stream")
                .And.HasMediaType("application/octet-stream"),
            (a) => a.HasName("Sync memory")
                .And.HasMediaType("application/octet-stream"),
            (a) => a.HasName("Sync text")
                .And.HasMediaType("text/plain"),
            (a) => a.HasName("Sync file")
                .And.HasMediaType("application/octet-stream"),
        ]);

        var stream = await Assert.That(testResult).HasAttachmentAt(0).That.HasSource();
        var memory = await Assert.That(testResult).HasAttachmentAt(1).That.HasSource();
        var text = await Assert.That(testResult).HasAttachmentAt(2).That.HasSource();
        var file = await Assert.That(testResult).HasAttachmentAt(3).That.HasSource();

        await Assert.That(results.Value.Attachments).ContainsKey(stream);
        await Assert.That(results.Value.Attachments[stream].ToArray())
            .IsEquivalentTo(new byte[] { 1, 2 ,3 }, CollectionOrdering.Matching);

        await Assert.That(results.Value.Attachments).ContainsKey(memory);
        await Assert.That(results.Value.Attachments[memory].ToArray())
            .IsEquivalentTo(new byte[] { 4, 5 }, CollectionOrdering.Matching);

        await Assert.That(results.Value.Attachments).ContainsKey(text);
        await Assert.That(results.Value.Attachments[text].ToArray())
            .IsEquivalentTo("Sync text body"u8.ToArray(), CollectionOrdering.Matching);

        await Assert.That(results.Value.Attachments).ContainsKey(file);
        await Assert.That(results.Value.Attachments[file].ToArray())
            .IsEquivalentTo(new byte[] { 6, 7 }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task AsyncRuntimeAttachmentsWork()
    {
        var testResult = await Assert.That(results.Value)
            .HasSingleTestResult(tr => tr.HasName().That.EndsWith(".AsyncRuntimeAttachments.TestMethod"))
            .With.Status(AllureStatus.Passed);

        await Assert.That(testResult).HasAttachmentsMatching([
            (a) => a.HasName("Async stream")
                .And.HasMediaType("application/octet-stream"),
            (a) => a.HasName("Async memory")
                .And.HasMediaType("application/octet-stream"),
            (a) => a.HasName("Async text")
                .And.HasMediaType("text/plain"),
            (a) => a.HasName("Async file")
                .And.HasMediaType("application/octet-stream"),
        ]);

        var stream = await Assert.That(testResult).HasAttachmentAt(0).That.HasSource();
        var memory = await Assert.That(testResult).HasAttachmentAt(1).That.HasSource();
        var text = await Assert.That(testResult).HasAttachmentAt(2).That.HasSource();
        var file = await Assert.That(testResult).HasAttachmentAt(3).That.HasSource();

        await Assert.That(results.Value.Attachments).ContainsKey(stream);
        await Assert.That(results.Value.Attachments[stream].ToArray())
            .IsEquivalentTo(new byte[] { 11, 12 ,13 }, CollectionOrdering.Matching);

        await Assert.That(results.Value.Attachments).ContainsKey(memory);
        await Assert.That(results.Value.Attachments[memory].ToArray())
            .IsEquivalentTo(new byte[] { 14, 15 }, CollectionOrdering.Matching);

        await Assert.That(results.Value.Attachments).ContainsKey(text);
        await Assert.That(results.Value.Attachments[text].ToArray())
            .IsEquivalentTo("Async text body"u8.ToArray(), CollectionOrdering.Matching);

        await Assert.That(results.Value.Attachments).ContainsKey(file);
        await Assert.That(results.Value.Attachments[file].ToArray())
            .IsEquivalentTo(new byte[] { 16, 17 }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SyncScreenDiffsWork()
    {
        var testResult = await Assert.That(results.Value)
            .HasSingleTestResult(tr => tr.HasName().That.EndsWith(".SyncScreenDiffs.TestMethod"))
            .With.Status(AllureStatus.Passed);

        await Assert.That(testResult).HasAttachmentsMatching([
            (a) => a.HasName("Screen diff 1")
                .And.HasMediaType("application/vnd.allure.image.diff"),
            (a) => a.HasName("Screen diff 2")
                .And.HasMediaType("application/vnd.allure.image.diff"),
        ]);

        var diff1 = await Assert.That(testResult).HasAttachmentAt(0).That.HasSource();
        var diff2 = await Assert.That(testResult).HasAttachmentAt(1).That.HasSource();

        await Assert.That(results.Value.Attachments).ContainsKey(diff1);
        await Assert.That(results.Value.Attachments).ContainsKey(diff2);

        using var diff1Document = JsonDocument.Parse(results.Value.Attachments[diff1]);

        await Assert.That(diff1Document.RootElement.GetProperty("expected").GetString())
            .IsEqualTo($"data:image/png;base64,AQ==");
        await Assert.That(diff1Document.RootElement.GetProperty("actual").GetString())
            .IsEqualTo($"data:image/png;base64,Ag==");
        await Assert.That(diff1Document.RootElement.GetProperty("diff").GetString())
            .IsEqualTo($"data:image/png;base64,Aw==");

        using var diff2Document = JsonDocument.Parse(results.Value.Attachments[diff2]);

        await Assert.That(diff2Document.RootElement.GetProperty("expected").GetString())
            .IsEqualTo($"data:image/png;base64,BA==");
        await Assert.That(diff2Document.RootElement.GetProperty("actual").GetString())
            .IsEqualTo($"data:image/png;base64,BQ==");
        await Assert.That(diff2Document.RootElement.GetProperty("diff").GetString())
            .IsEqualTo($"data:image/png;base64,Bg==");
    }

    [Test]
    public async Task AsyncScreenDiffsWork()
    {
        var testResult = await Assert.That(results.Value)
            .HasSingleTestResult(tr => tr.HasName().That.EndsWith(".AsyncScreenDiffs.TestMethod"))
            .With.Status(AllureStatus.Passed);

        await Assert.That(testResult).HasAttachmentsMatching([
            (a) => a.HasName("Screen diff 1")
                .And.HasMediaType("application/vnd.allure.image.diff"),
            (a) => a.HasName("Screen diff 2")
                .And.HasMediaType("application/vnd.allure.image.diff"),
        ]);

        var diff1 = await Assert.That(testResult).HasAttachmentAt(0).That.HasSource();
        var diff2 = await Assert.That(testResult).HasAttachmentAt(1).That.HasSource();

        await Assert.That(results.Value.Attachments).ContainsKey(diff1);
        await Assert.That(results.Value.Attachments).ContainsKey(diff2);

        using var diff1Document = JsonDocument.Parse(results.Value.Attachments[diff1]);

        await Assert.That(diff1Document.RootElement.GetProperty("expected").GetString())
            .IsEqualTo($"data:image/png;base64,Cw==");
        await Assert.That(diff1Document.RootElement.GetProperty("actual").GetString())
            .IsEqualTo($"data:image/png;base64,DA==");
        await Assert.That(diff1Document.RootElement.GetProperty("diff").GetString())
            .IsEqualTo($"data:image/png;base64,DQ==");

        using var diff2Document = JsonDocument.Parse(results.Value.Attachments[diff2]);

        await Assert.That(diff2Document.RootElement.GetProperty("expected").GetString())
            .IsEqualTo($"data:image/png;base64,Dg==");
        await Assert.That(diff2Document.RootElement.GetProperty("actual").GetString())
            .IsEqualTo($"data:image/png;base64,Dw==");
        await Assert.That(diff2Document.RootElement.GetProperty("diff").GetString())
            .IsEqualTo($"data:image/png;base64,EA==");
    }

    [Test]
    public async Task ContentAttachmentAttributesWork()
    {
        var testResult = await Assert.That(results.Value)
            .HasSingleTestResult(tr => tr.HasName().That.EndsWith(".ContentAttachmentAttributes.TestMethod"))
            .With.Status(AllureStatus.Passed);

        await Assert.That(testResult).HasAttachmentsMatching([
            (a) => a.HasName("Attribute text")
                .And.HasMediaType("text/plain"),
            (a) => a.HasName("Async attribute bytes")
                .And.HasMediaType("application/octet-stream"),
        ]);

        var stream = await Assert.That(testResult).HasAttachmentAt(0).That.HasSource();
        var memory = await Assert.That(testResult).HasAttachmentAt(1).That.HasSource();

        await Assert.That(results.Value.Attachments).ContainsKey(stream);
        await Assert.That(results.Value.Attachments[stream].ToArray())
            .IsEquivalentTo("Attribute text body"u8.ToArray(), CollectionOrdering.Matching);

        await Assert.That(results.Value.Attachments).ContainsKey(memory);
        await Assert.That(results.Value.Attachments[memory].ToArray())
            .IsEquivalentTo(new byte[] { 21, 22 }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task FileAttachmentAttributesWork()
    {
        var testResult = await Assert.That(results.Value)
            .HasSingleTestResult(tr => tr.HasName().That.EndsWith(".FileAttachmentAttributes.TestMethod"))
            .With.Status(AllureStatus.Passed);

        await Assert.That(testResult).HasAttachmentsMatching([
            (a) => a.HasName("Attribute file")
                .And.HasMediaType("application/octet-stream"),
            (a) => a.HasName("Async attribute file")
                .And.HasMediaType("application/octet-stream"),
        ]);

        var stream = await Assert.That(testResult).HasAttachmentAt(0).That.HasSource();
        var memory = await Assert.That(testResult).HasAttachmentAt(1).That.HasSource();

        await Assert.That(results.Value.Attachments).ContainsKey(stream);
        await Assert.That(results.Value.Attachments[stream].ToArray())
            .IsEquivalentTo(new byte[] { 31, 32 }, CollectionOrdering.Matching);

        await Assert.That(results.Value.Attachments).ContainsKey(memory);
        await Assert.That(results.Value.Attachments[memory].ToArray())
            .IsEquivalentTo(new byte[] { 33, 34 }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task XunitAttachmentsWork()
    {
        var testResult = await Assert.That(results.Value)
            .HasSingleTestResult(tr => tr.HasName().That.EndsWith(".XunitAttachments.TestMethod"))
            .With.Status(AllureStatus.Passed);

        await Assert.That(testResult).HasAttachmentsMatching([
            (a) => a.HasName("xUnit binary"),
            (a) => a.HasName("xUnit text"),
            (a) => a.HasName("Standard output"),
        ]);

        var binary = await Assert.That(testResult).HasAttachmentAt(0).That.HasSource();
        var text = await Assert.That(testResult).HasAttachmentAt(1).That.HasSource();
        var stdout = await Assert.That(testResult).HasAttachmentAt(2).That.HasSource();

        await Assert.That(results.Value.Attachments).ContainsKey(text);
        await Assert.That(results.Value.Attachments[text].ToArray())
            .IsEquivalentTo("xUnit text body"u8.ToArray(), CollectionOrdering.Matching);

        await Assert.That(results.Value.Attachments).ContainsKey(binary);
        await Assert.That(results.Value.Attachments[binary].ToArray())
            .IsEquivalentTo(new byte[] { 41, 42, 43 }, CollectionOrdering.Matching);

        await Assert.That(results.Value.Attachments).ContainsKey(stdout);
        await Assert.That(results.Value.Attachments[stdout].ToArray())
            .IsEquivalentTo("stdout content"u8.ToArray(), CollectionOrdering.Matching);
    }
}
