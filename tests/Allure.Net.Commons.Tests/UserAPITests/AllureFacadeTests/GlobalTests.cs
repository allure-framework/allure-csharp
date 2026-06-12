using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserApiTests.AllureFacadeTests;

internal class GlobalTests : AllureApiTestFixture
{
    [Test]
    public void GlobalAttachmentFromBytesWritesAttachmentAndGlobalsChunk()
    {
        byte[] expectedContent = [1, 2, 3];

        var timestampBefore = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        AllureApi.AddGlobalAttachment("global", "text/plain", expectedContent, ".txt");
        var timestampAfter = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        Assert.That(this.writer.attachments, Has.One.Items);
        Assert.That(this.writer.globals, Has.One.Items);

        var global = this.writer.globals[0];
        var (actualSource, actualContent) = this.writer.attachments[0];

        Assert.That(global.attachments, Has.One.Items);
        Assert.That(global.errors, Is.Empty);

        var globalAttachment = global.attachments[0];

        Assert.That(globalAttachment.name, Is.EqualTo("global"));
        Assert.That(globalAttachment.type, Is.EqualTo("text/plain"));
        Assert.That(globalAttachment.source, Does.EndWith(".txt"));
        Assert.That(
            globalAttachment.timestamp,
            Is.GreaterThanOrEqualTo(timestampBefore)
                .And.LessThanOrEqualTo(timestampAfter));
        Assert.That(actualSource, Is.EqualTo(globalAttachment.source));
        Assert.That(actualContent, Is.EqualTo(expectedContent));
    }

    [Test]
    public void GlobalAttachmentFromPathUsesFileNameAndMimeType()
    {
        var path = Path.Combine("foo", "bar.txt");

        AllureApi.AddGlobalAttachment(path);

        var globalAttachment = this.writer.globals.Single().attachments.Single();
        Assert.That(globalAttachment.name, Is.EqualTo("bar.txt"));
        Assert.That(globalAttachment.type, Is.EqualTo("text/plain"));
        Assert.That(globalAttachment.source, Does.EndWith(".txt"));
        var actualAbsolutePath = this.writer.attachmentFiles.Single().Path;
        var actualRelativePath = Path.GetRelativePath(Environment.CurrentDirectory, actualAbsolutePath);
        Assert.That(actualRelativePath, Is.EqualTo(path));
    }

    [Test]
    public void GlobalErrorFromExceptionWritesGlobalsChunk()
    {
        var error = new InvalidOperationException("boom");

        var timestampBefore = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        AllureApi.AddGlobalError(error);
        var timestampAfter = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        var global = this.writer.globals.Single();
        Assert.That(global.attachments, Is.Empty);

        var globalError = global.errors.Single();
        Assert.That(globalError.message, Is.EqualTo("boom"));
        Assert.That(globalError.trace, Does.Contain(nameof(InvalidOperationException)));
        Assert.That(
            globalError.timestamp,
            Is.GreaterThanOrEqualTo(timestampBefore)
                .And.LessThanOrEqualTo(timestampAfter));
    }

    [Test]
    public void GlobalErrorFromMessageWritesMessageOnly()
    {
        AllureApi.AddGlobalError("boom");

        var globalError = this.writer.globals.Single().errors.Single();
        Assert.That(globalError.message, Is.EqualTo("boom"));
        Assert.That(globalError.trace, Is.Null);
        Assert.That(globalError.timestamp, Is.GreaterThan(0));
    }

    [Test]
    public void GlobalErrorFromStatusDetailsPreservesProvidedFields()
    {
        var details = new StatusDetails
        {
            message = "boom",
            trace = "stack",
            known = true,
            muted = true,
            flaky = true
        };

        AllureApi.AddGlobalError(details);

        var globalError = this.writer.globals.Single().errors.Single();
        Assert.That(globalError.message, Is.EqualTo("boom"));
        Assert.That(globalError.trace, Is.EqualTo("stack"));
        Assert.That(globalError.known, Is.True);
        Assert.That(globalError.muted, Is.True);
        Assert.That(globalError.flaky, Is.True);
    }

    [Test]
    public void GlobalCallsProduceSeparateChunks()
    {
        AllureApi.AddGlobalError("one");
        AllureApi.AddGlobalError("two");

        var globals = this.writer.globals;

        Assert.That(globals, Has.Count.EqualTo(2));
        Assert.That(globals.All(g => g.errors.Count == 1));
        Assert.That(
            globals.Select(g => g.errors[0].message),
            Is.EquivalentTo(["one", "two"]));
    }

    [Test]
    public void GlobalApisWorkWithoutActiveTestContext()
    {
        AllureApi.AddGlobalAttachment("global", "text/plain", [1], ".txt");
        AllureApi.AddGlobalError("boom");

        Assert.That(this.writer.attachments, Has.One.Items);
        Assert.That(this.writer.globals, Has.Count.EqualTo(2));
    }

    [Test]
    public void GlobalApisDoNotMutateCurrentTestAttachments()
    {
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });

        AllureApi.AddGlobalAttachment("global", "text/plain", [1], ".txt");

        Assert.That(this.Context.CurrentTest.attachments, Is.Empty);
        Assert.That(this.writer.globals, Has.One.Items);
    }
}