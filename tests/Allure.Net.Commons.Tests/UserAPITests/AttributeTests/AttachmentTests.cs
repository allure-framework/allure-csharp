using System;
using System.IO;
using System.Threading.Tasks;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.Tests.UserApiTests;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.UserAPITests.AttributeTests;

class AttachmentTests : AllureApiTestFixture
{
    TestResult testResult;

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
    }

    [Test]
    public void CreatesAttachmentFromByteArray()
    {
        AttachByteArray();

        Assert.That(this.testResult.attachments, Has.One.Items);
        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo(nameof(AttachByteArray)));
        Assert.That(attachment.type, Is.Null);
        Assert.That(attachment.source, Does.Not.Contain("."));
        Assert.That(this.writer.attachments, Contains.Item((attachment.source, new byte[]{ 1, 2, 3 })));
    }

    [Test]
    public void CreatesAttachmentFromString()
    {
        AttachString();

        Assert.That(this.testResult.attachments, Has.One.Items);
        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo(nameof(AttachString)));
        Assert.That(attachment.type, Is.EqualTo("text/plain"));
        Assert.That(attachment.source, Does.EndWith(".txt"));
        Assert.That(
            this.writer.attachments,
            Contains.Item((
                attachment.source,
                "Lorem Ipsum"u8.ToArray())));
    }

    [Test]
    public void CreatesAttachmentFromStream()
    {
        using var stream = AttachStream();

        Assert.That(this.testResult.attachments, Has.One.Items);
        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo(nameof(AttachStream)));
        Assert.That(attachment.type, Is.Null);
        Assert.That(attachment.source, Does.Not.Contain("."));
        Assert.That(
            this.writer.attachments,
            Contains.Item((
                attachment.source,
                new byte[]{ 1, 2, 3 })));
        Assert.That(stream.Position, Is.Zero);
    }

    [Test]
    public void ThrowsIfStreamCanNotRead()
    {
        Assert.That(
            AttachNonReadableStream,
            Throws.InstanceOf<InvalidOperationException>()
                .With.Message.EqualTo(
                    $"Can't create an Allure attachment from {typeof(NonReadableStream).FullName}: "
                        + "this stream does not support the read operation."
                )
        );
    }

    [Test]
    public void ThrowsIfStreamCanNotSeek()
    {
        Assert.That(
            AttachNonSeekableStream,
            Throws.InstanceOf<InvalidOperationException>()
                .With.Message.EqualTo(
                    $"Can't create an Allure attachment from {typeof(NonSeekableStream).FullName}: "
                        + "this stream does not support the seek operation."
                )
        );
    }

    [Test]
    public void ThrowsIfTypeNotSupported()
    {
        Assert.That(
            AttachInt,
            Throws.InstanceOf<InvalidOperationException>()
                .With.Message.EqualTo(
                    "Can't create an Allure attachment from System.Int32. "
                        + "A string, byte[], or stream was expected."
                )
        );
    }

    [Test]
    public void UsesEncodingToConvertStrings()
    {
        AttachEncoding();

        Assert.That(
            this.writer.attachments,
            Contains.Item((
                this.testResult.attachments[0].source,
                new byte[]
                {
                    0x4c, 0x00, 0x6f, 0x00, 0x72, 0x00, 0x65, 0x00, 0x6d, 0x00, 0x20, 0x00,
                    0x49, 0x00, 0x70, 0x00, 0x73, 0x00, 0x75, 0x00, 0x6D, 0x00
                })));
    }

    [Test]
    public void UsesContentType()
    {
        AttachJson();

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.type, Is.EqualTo("application/json"));
        Assert.That(attachment.source, Does.EndWith(".json"));
    }

    [Test]
    public void UsesExtension()
    {
        AttachExtension();

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.source, Does.EndWith(".foo"));
    }

    [Test]
    public void AddsDotBeforeExtension()
    {
        AttachExtensionNoDot();

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.source, Does.EndWith(".foo"));
    }

    [Test]
    public void DoesntAddDotBeforeExtensionIfAlreadyStartsWithDot()
    {
        AttachExtension();

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.source, Does.Not.EndWith("..foo"));
    }

    [Test]
    public void AppendsNothingIfExtensionIsEmpty()
    {
        AttachEmptyExtension();

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.source, Does.Not.Contains("."));
    }

    [Test]
    public void UsesExplicitName()
    {
        AttachName();

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo("Foo"));
    }

    [Test]
    public void InterpolatesArgumentsIntoName()
    {
        AttachInterpolatedName(1, "foo");

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo("1 \"foo\""));
    }

    [Test]
    public void UsesTypeFormatters()
    {
        AttachCustomFormatter(new());

        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo("foo"));
    }

    [Test]
    public async Task SupportsAsyncFunctions()
    {
        await AttachStringAsync();

        Assert.That(this.testResult.attachments, Has.One.Items);
        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo(nameof(AttachStringAsync)));
        Assert.That(attachment.type, Is.EqualTo("text/plain"));
        Assert.That(attachment.source, Does.EndWith(".txt"));
        Assert.That(
            this.writer.attachments,
            Contains.Item((
                attachment.source,
                "Lorem Ipsum"u8.ToArray())));
    }

    [Test]
    public async Task SupportsValueTask()
    {
        await AttachStringValueTask();

        Assert.That(this.testResult.attachments, Has.One.Items);
        var attachment = this.testResult.attachments[0];
        Assert.That(attachment.name, Is.EqualTo(nameof(AttachStringValueTask)));
        Assert.That(attachment.type, Is.EqualTo("text/plain"));
        Assert.That(attachment.source, Does.EndWith(".txt"));
        Assert.That(
            this.writer.attachments,
            Contains.Item((
                attachment.source,
                "Lorem Ipsum"u8.ToArray())));
    }

    [Test]
    public void NoEffectIfNoContextActive()
    {
        this.lifecycle.StopTestCase();
        this.lifecycle.WriteTestCase();

        Assert.That(() => AttachFailedFormatter(new()), Throws.Nothing);
    }

    [Test]
    public void CreatesGlobalAttachmentIfConfiguredWithoutContext()
    {
        this.lifecycle.StopTestCase();
        this.lifecycle.WriteTestCase();

        AttachGlobalString();

        var globals = this.writer.globals;
        Assert.That(globals, Has.One.Items);

        var globalAttachments = globals[0].attachments;
        Assert.That(globalAttachments, Has.One.Items);

        var globalAttachment = globalAttachments[0];
        Assert.That(globalAttachment.name, Is.EqualTo(nameof(AttachGlobalString)));
        Assert.That(globalAttachment.type, Is.EqualTo("text/plain"));
    }

    [Test]
    public void CreatesGlobalAttachmentInsteadOfTestAttachmentIfConfigured()
    {
        AttachGlobalString();

        Assert.That(this.testResult.attachments, Is.Empty);
        Assert.That(this.writer.globals, Has.One.Items);
        Assert.That(this.writer.globals[0].attachments, Has.One.Items);
    }

    [AllureAttachment]
    static byte[] AttachByteArray() => [1, 2, 3];

    [AllureAttachment]
    static string AttachString() => "Lorem Ipsum";

    [AllureAttachment]
    static Stream AttachStream() => new MemoryStream([1, 2, 3]);

    class NonSeekableStream(byte[] buffer) : MemoryStream(buffer)
    {
        public override bool CanSeek => false;
    }

    [AllureAttachment]
    static Stream AttachNonSeekableStream() => new NonSeekableStream([]);

    class NonReadableStream(byte[] buffer) : MemoryStream(buffer)
    {
        public override bool CanRead => false;
    }

    [AllureAttachment]
    static Stream AttachNonReadableStream() => new NonReadableStream([]);

    [AllureAttachment]
    static int AttachInt() => 1;

    [AllureAttachment(Encoding = "UTF-16")]
    static string AttachEncoding() => "Lorem Ipsum";

    [AllureAttachment(ContentType = "application/json")]
    static byte[] AttachJson() => [];

    [AllureAttachment(Extension = ".foo")]
    static byte[] AttachExtension() => [];

    [AllureAttachment(Extension = "foo")]
    static byte[] AttachExtensionNoDot() => [];

    [AllureAttachment(Extension = "")]
    static byte[] AttachEmptyExtension() => [];

    [AllureAttachment("Foo")]
    static byte[] AttachName() => [];

    [AllureAttachment("{arg1} {arg2}")]
    static byte[] AttachInterpolatedName(int arg1, string arg2) => [];

    [AllureAttachment]
    static async Task<string> AttachStringAsync()
    {
        await Task.Yield();
        return "Lorem Ipsum";
    }

    [AllureAttachment]
    static async ValueTask<string> AttachStringValueTask()
    {
        await Task.Yield();
        return "Lorem Ipsum";
    }

    [AllureAttachment(Global = true)]
    static string AttachGlobalString() => "Lorem Ipsum";

    class InterpolationStub
    {
        public class TF : TypeFormatter<InterpolationStub>
        {
            public override string Format(InterpolationStub value) => "foo";
        }
    }

    [AllureAttachment("{arg}")]
    static byte[] AttachCustomFormatter(InterpolationStub arg) => [];

    class InterpolationDummy
    {
        public class TF : TypeFormatter<InterpolationDummy>
        {
            public override string Format(InterpolationDummy value)
                => throw new NotImplementedException();
        }
    }

    [AllureAttachment("{arg}")]
    static byte[] AttachFailedFormatter(InterpolationDummy arg) => [];
}
