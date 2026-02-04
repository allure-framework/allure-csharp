using System;
using System.IO;
using System.Threading.Tasks;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.Tests.UserApiTests;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AttributeTests;

class AttachmentFileTests : AllureApiTestFixture
{
    TestResult testResult;
    DirectoryInfo dir;

    [SetUp]
    public void SetUpContext()
    {
        this.lifecycle.AddTypeFormatter(new InterpolationStub.TF());
        this.lifecycle.AddTypeFormatter(new InterpolationDummy.TF());
        this.testResult = new() {
            uuid = IdFunctions.CreateUUID(),
            fullName = "foo",
        };
        this.lifecycle.StartTestCase(this.testResult);
        this.dir = Directory.CreateTempSubdirectory("allure-");
    }

    [TearDown]
    public void DeleteFiles()
    {
        this.dir.Delete(true);
    }

    FileInfo CreateFile(string name = null, byte[] content = null)
    {
        name ??= Guid.NewGuid().ToString();
        content ??= [];
        var path = Path.Combine(this.dir.FullName, name);
        File.WriteAllBytes(path, content);
        return new FileInfo(path);
    }

    [Test]
    public void CreatesAttachmentFromStringPath()
    {
        this.AttachByStringPath();

        Assert.That(this.testResult.attachments, Has.One.Items);
        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo("foo"));
        Assert.That(attachment.type, Is.Null);
        Assert.That(attachment.source, Does.Not.Contain("."));
        Assert.That(this.writer.attachments, Contains.Item((attachment.source, new byte[]{ 1, 2, 3 })));
    }

    [Test]
    public void CreatesAttachmentFromFileInfo()
    {
        this.AttachByFileInfo();

        Assert.That(this.testResult.attachments, Has.One.Items);
        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo("foo"));
        Assert.That(attachment.type, Is.Null);
        Assert.That(attachment.source, Does.Not.Contain("."));
        Assert.That(this.writer.attachments, Contains.Item((attachment.source, new byte[]{ 1, 2, 3 })));
    }

    [Test]
    public void NoEffectIfNull()
    {
        Assert.That(this.AttachByNull, Throws.Nothing);

        Assert.That(this.testResult.attachments, Is.Empty);
        Assert.That(this.writer.attachments, Is.Empty);
    }

    [Test]
    public void ThrowsIfTypeNotSupported()
    {
        Assert.That(
            this.AttachByInt,
            Throws.InstanceOf<InvalidOperationException>()
                .With.Message.EqualTo(
                    "Can't create an Allure file attachment from System.Int32. "
                        + "A string or System.IO.FileInfo was expected."
                )
        );
    }

    [Test]
    public void UsesFileExtension()
    {
        this.AttachWithExtension();

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.source, Does.EndWith(".bar"));
    }

    [Test]
    public void UsesContentTypeAndOriginalExtension()
    {
        this.AttachWithContentTypeAndExtension();

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.type, Is.EqualTo("application/json"));
        Assert.That(attachment.source, Does.EndWith(".bar"));
    }

    [Test]
    public void SetsExtensionFromContentTypeIfNotPresent()
    {
        this.AttachWithContentTypeAndNoExtension();

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.source, Does.EndWith(".json"));
    }

    [Test]
    public void UsesExplicitName()
    {
        this.AttachWithName();

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo("Foo"));
    }

    [Test]
    public void InterpolatesArgumentsIntoName()
    {
        this.AttachWithInterpolation(1, "foo");

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo("1 \"foo\""));
    }

    [Test]
    public void UsesTypeFormatters()
    {
        this.AttachWithCustomFormatter(new());

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo("foo"));
    }

    [Test]
    public async Task SupportsAsyncFunctions()
    {
        await this.AttachViaTask();

        var attachment = this.testResult.attachments[0];
        Assert.That(
            this.writer.attachments,
            Contains.Item((
                attachment.source,
                new byte[]{ 1, 2, 3 })));
    }

    [Test]
    public async Task SupportsValueTask()
    {
        await this.AttachViaValueTask();

        var attachment = this.testResult.attachments[0];
        Assert.That(
            this.writer.attachments,
            Contains.Item((
                attachment.source,
                new byte[]{ 1, 2, 3 })));
    }

    [Test]
    public void NoEffectIfNoContextActive()
    {
        this.lifecycle.StopTestCase();
        this.lifecycle.WriteTestCase();

        Assert.That(() => this.AttachWithFailedFormatter(new()), Throws.Nothing);
    }

    [AllureAttachmentFile]
    string AttachByStringPath() => this.CreateFile("foo", [1,2,3]).FullName;

    [AllureAttachmentFile]
    FileInfo AttachByFileInfo() => this.CreateFile("foo", [1,2,3]);

    [AllureAttachmentFile]
    string AttachByNull() => null;

    [AllureAttachmentFile]
    int AttachByInt() => 1;

    [AllureAttachmentFile]
    FileInfo AttachWithExtension() => this.CreateFile("foo.bar");

    [AllureAttachmentFile(ContentType = "application/json")]
    FileInfo AttachWithContentTypeAndExtension() => this.CreateFile("foo.bar");

    [AllureAttachmentFile(ContentType = "application/json")]
    FileInfo AttachWithContentTypeAndNoExtension() => this.CreateFile("foo");

    [AllureAttachmentFile("Foo")]
    FileInfo AttachWithName() => this.CreateFile();

    [AllureAttachmentFile("{arg1} {arg2}")]
    FileInfo AttachWithInterpolation(int arg1, string arg2) => this.CreateFile();

    [AllureAttachmentFile]
    async Task<FileInfo> AttachViaTask()
    {
        await Task.Yield();
        return this.CreateFile("foo.baz", [1, 2, 3]);
    }

    [AllureAttachmentFile]
    async ValueTask<FileInfo> AttachViaValueTask()
    {
        await Task.Yield();
        return this.CreateFile("foo.baz", [1, 2, 3]);
    }

    class InterpolationStub
    {
        public class TF : TypeFormatter<InterpolationStub>
        {
            public override string Format(InterpolationStub value) => "foo";
        }
    }

    [AllureAttachmentFile("{arg}")]
    FileInfo AttachWithCustomFormatter(InterpolationStub arg) => this.CreateFile();

    class InterpolationDummy
    {
        public class TF : TypeFormatter<InterpolationDummy>
        {
            public override string Format(InterpolationDummy value)
                => throw new NotImplementedException();
        }
    }

    [AllureAttachmentFile("{arg}")]
    FileInfo AttachWithFailedFormatter(InterpolationDummy arg) => this.CreateFile();
}
