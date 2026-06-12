using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserApiTests.AllureFacadeTests;

internal class AttachmentTests : AllureApiTestFixture
{
    [Test]
    public void ShouldAddAttachmentFileWithNoType()
    {
        var path = Path.Combine("foo", "bar");
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });

        AllureApi.AddAttachment(path);

        var attachment = this.Context.CurrentTest.attachments.Single();

        Assert.That(attachment.name, Is.EqualTo("bar"));
        Assert.That(attachment.type, Is.Null);
        Assert.That(attachment.source, Does.EndWith("-attachment"));

        var (outputName, sourceAbsolutePath) = this.writer.attachmentFiles.Single();
        Assert.That(outputName, Is.EqualTo(attachment.source));
        Assert.That(Path.GetRelativePath(Environment.CurrentDirectory, sourceAbsolutePath), Is.EqualTo(path));
    }

    [Test]
    public void ShouldAddAttachmentFileWithTypeFromExtension()
    {
        var path = Path.Combine("foo", "bar.json");
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });

        AllureApi.AddAttachment(path);

        var attachment = this.Context.CurrentTest.attachments.Single();

        Assert.That(attachment.name, Is.EqualTo("bar.json"));
        Assert.That(attachment.type, Is.EqualTo("application/json"));
        Assert.That(attachment.source, Does.EndWith("-attachment.json"));

        var (outputName, sourceAbsolutePath) = this.writer.attachmentFiles.Single();
        Assert.That(outputName, Is.EqualTo(attachment.source));
        Assert.That(Path.GetRelativePath(Environment.CurrentDirectory, sourceAbsolutePath), Is.EqualTo(path));
    }

    [Test]
    public void ShouldAddAttachmentFileWithExplicitName()
    {
        var path = Path.Combine("foo", "bar");
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });

        AllureApi.AddAttachment(path, "new name");

        var attachment = this.Context.CurrentTest.attachments.Single();

        Assert.That(attachment.name, Is.EqualTo("new name"));
        Assert.That(attachment.type, Is.Null);
        Assert.That(attachment.source, Does.EndWith("-attachment"));

        var (outputName, sourceAbsolutePath) = this.writer.attachmentFiles.Single();
        Assert.That(outputName, Is.EqualTo(attachment.source));
        Assert.That(Path.GetRelativePath(Environment.CurrentDirectory, sourceAbsolutePath), Is.EqualTo(path));
    }

    [Test]
    public void ShouldAddAttachmentFileWithExplicitNameAndType()
    {
        var path = Path.Combine("foo", "bar");
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });

        AllureApi.AddAttachment("new name", "text/plain", path);

        var attachment = this.Context.CurrentTest.attachments.Single();

        Assert.That(attachment.name, Is.EqualTo("new name"));
        Assert.That(attachment.type, Is.EqualTo("text/plain"));
        Assert.That(attachment.source, Does.EndWith("-attachment"));

        var (outputName, sourceAbsolutePath) = this.writer.attachmentFiles.Single();
        Assert.That(outputName, Is.EqualTo(attachment.source));
        Assert.That(Path.GetRelativePath(Environment.CurrentDirectory, sourceAbsolutePath), Is.EqualTo(path));
    }

    [Test]
    public void ShouldAddAttachmentContent()
    {
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });

        AllureApi.AddAttachment("foo", "text/plain", [1, 2, 3]);

        var attachment = this.Context.CurrentTest.attachments.Single();

        Assert.That(attachment.name, Is.EqualTo("foo"));
        Assert.That(attachment.type, Is.EqualTo("text/plain"));
        Assert.That(attachment.source, Does.EndWith("-attachment"));

        var (outputName, content) = this.writer.attachments.Single();
        Assert.That(outputName, Is.EqualTo(attachment.source));
        Assert.That(content, Is.EqualTo([1, 2, 3]));
    }

    [Test]
    public void ShouldAddAttachmentContentWithExtension()
    {
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });

        AllureApi.AddAttachment("foo", "text/plain", [1, 2, 3], ".txt");

        var attachment = this.Context.CurrentTest.attachments.Single();

        Assert.That(attachment.name, Is.EqualTo("foo"));
        Assert.That(attachment.type, Is.EqualTo("text/plain"));
        Assert.That(attachment.source, Does.EndWith("-attachment.txt"));

        var (outputName, content) = this.writer.attachments.Single();
        Assert.That(outputName, Is.EqualTo(attachment.source));
        Assert.That(content, Is.EqualTo([1, 2, 3]));
    }

    [Test]
    public void ScreenDiffTest()
    {
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });
        var expectedExpected = File.ReadAllBytes("expected.png");
        var expectedActual = File.ReadAllBytes("actual.png");
        var expectedDiff = File.ReadAllBytes("diff.png");

        AllureApi.AddScreenDiff("expected.png", "actual.png", "diff.png");

        var attachment = this.Context.CurrentTest.attachments.Single();
        var content = JsonConvert.DeserializeAnonymousType(
            Encoding.UTF8.GetString(
                this.writer.attachments.Single().Content
            ),
            new { expected = "", actual = "", diff = "" }
        );
        var prefix = "data:image/png;base64,";
        var actualExpected = Convert.FromBase64String(
            content.expected[prefix.Length..]
        );
        var actualActual = Convert.FromBase64String(
            content.actual[prefix.Length..]
        );
        var actualDiff = Convert.FromBase64String(
            content.diff[prefix.Length..]
        );

        Assert.That(attachment.name, Is.EqualTo("diff-1"));
        Assert.That(attachment.type, Is.EqualTo("application/vnd.allure.image.diff"));
        Assert.That(attachment.source, Does.EndWith(".json"));
        Assert.That(content.expected, Does.StartWith(prefix));
        Assert.That(content.actual, Does.StartWith(prefix));
        Assert.That(content.diff, Does.StartWith(prefix));
        Assert.That(actualExpected, Is.EqualTo(expectedExpected));
        Assert.That(actualActual, Is.EqualTo(expectedActual));
        Assert.That(actualDiff, Is.EqualTo(expectedDiff));
    }

    [Test]
    public void ScreenDiffNameIncremented()
    {
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });
        AllureApi.AddScreenDiff("expected.png", "actual.png", "diff.png");

        AllureApi.AddScreenDiff("expected.png", "actual.png", "diff.png");

        var name = this.Context.CurrentTest.attachments.Last().name;
        Assert.That(name, Is.EqualTo("diff-2"));
    }

    [Test]
    public void StepScreenDiff()
    {
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });
        ExtendedApi.StartStep("step");

        AllureApi.AddScreenDiff("expected.png", "actual.png", "diff.png");

        Assert.That(
            this.Context.CurrentStep.attachments,
            Is.Not.Empty
        );
    }

    [Test]
    public void FixtureScreenDiff()
    {
        this.lifecycle.StartTestContainer(new() { uuid = "2" });
        ExtendedApi.StartBeforeFixture("fixture");

        AllureApi.AddScreenDiff("expected.png", "actual.png", "diff.png");

        Assert.That(
            this.Context.CurrentFixture.attachments,
            Is.Not.Empty
        );
    }

    [Test]
    public void ScreenDiffFromBytes()
    {
        this.lifecycle.StartTestCase(new() { uuid = "1", fullName = "n" });
        byte[] expectedExpected = [1, 2, 3];
        byte[] expectedActual = [4, 5, 6];
        byte[] expectedDiff = [7, 8, 9];

        AllureApi.AddScreenDiff(expectedExpected, expectedActual, expectedDiff);

        var attachment = this.Context.CurrentTest.attachments.Single();
        var content = JsonConvert.DeserializeAnonymousType(
            Encoding.UTF8.GetString(
                this.writer.attachments.Single().Content
            ),
            new { expected = "", actual = "", diff = "" }
        );
        var prefix = "data:image/png;base64,";
        var actualExpected = Convert.FromBase64String(
            content.expected[prefix.Length..]
        );
        var actualActual = Convert.FromBase64String(
            content.actual[prefix.Length..]
        );
        var actualDiff = Convert.FromBase64String(
            content.diff[prefix.Length..]
        );

        Assert.That(attachment.name, Is.EqualTo("diff-1"));
        Assert.That(attachment.type, Is.EqualTo("application/vnd.allure.image.diff"));
        Assert.That(attachment.source, Does.EndWith(".json"));
        Assert.That(content.expected, Does.StartWith(prefix));
        Assert.That(content.actual, Does.StartWith(prefix));
        Assert.That(content.diff, Does.StartWith(prefix));
        Assert.That(actualExpected, Is.EqualTo(expectedExpected));
        Assert.That(actualActual, Is.EqualTo(expectedActual));
        Assert.That(actualDiff, Is.EqualTo(expectedDiff));
    }
}
