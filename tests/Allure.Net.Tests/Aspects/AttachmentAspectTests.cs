using System.Reflection;
using System.Text;
using Allure.Abstractions;
using Allure.Aspects;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Aspects;

public class AttachmentAspectTests
{
    [Test]
    public async Task TextAttachmentAppliesMetadataNameEncodingAndExtension()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        byte[]? capturedContent = null;
        operations.Handler = (_, arguments) =>
        {
            capturedContent = Read((Stream)arguments[1]!);
            return null;
        };
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(
            sync: operations.Instance,
            serializer: new TestParameterSerializer("name")
        ));

        new AllureAttachmentAspect().AttachReturnValue(
            nameof(TextAttachment),
            Method(nameof(TextAttachment)),
            [42],
            typeof(string),
            "content"
        );

        var call = operations.SingleCall;
        await Assert.That(call.Method.Name).IsEqualTo("AddAttachment");
        await Assert.That(call.Arguments[0]).IsEqualTo("attachment name:42");
        await Assert.That(call.Arguments[2]).IsEqualTo("application/example");
        await Assert.That(call.Arguments[3]).IsEqualTo(".data");
        await Assert.That(capturedContent).IsEquivalentTo(Encoding.Unicode.GetBytes("content"));
    }

    [Test]
    public async Task DefaultTextAttachmentUsesMethodNameAndTextMediaType()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        operations.Handler = (_, arguments) => { Read((Stream)arguments[1]!); return null; };
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));

        new AllureAttachmentAspect().AttachReturnValue(
            nameof(DefaultTextAttachment), Method(nameof(DefaultTextAttachment)), [], typeof(string), "text"
        );

        await Assert.That(operations.SingleCall.Arguments[0]).IsEqualTo(nameof(DefaultTextAttachment));
        await Assert.That(operations.SingleCall.Arguments[2]).IsEqualTo("text/plain");
        await Assert.That(operations.SingleCall.Arguments[3]).IsEqualTo(".txt");
    }

    [Test]
    public async Task GlobalAttachmentUsesGlobalEndpoint()
    {
        var current = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        var global = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        global.Handler = (_, arguments) => { Read((Stream)arguments[1]!); return null; };
        using var scope = FacadeTestEnvironment.Use(
            new TestApiEndpoint(sync: current.Instance),
            new TestApiEndpoint(sync: global.Instance)
        );

        new AllureAttachmentAspect().AttachReturnValue(
            nameof(GlobalAttachment), Method(nameof(GlobalAttachment)), [], typeof(byte[]), new byte[] { 1, 2 }
        );

        await Assert.That(current.Calls).IsEmpty();
        await Assert.That(global.SingleCall.Method.Name).IsEqualTo("AddGlobalAttachment");
        await Assert.That(scope.GlobalResolutionCount).IsEqualTo(1);
        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(0);
    }

    [Test]
    public async Task StreamAttachmentConsumesRemainingContentAndRestoresPosition()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        byte[]? capturedContent = null;
        operations.Handler = (_, arguments) => { capturedContent = Read((Stream)arguments[1]!); return null; };
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));
        using var content = new MemoryStream([1, 2, 3, 4]);
        content.Position = 2;

        new AllureAttachmentAspect().AttachReturnValue(
            nameof(DefaultTextAttachment), Method(nameof(DefaultTextAttachment)), [], typeof(Stream), content
        );

        await Assert.That(capturedContent).IsEquivalentTo(new byte[] { 3, 4 });
        await Assert.That(content.Position).IsEqualTo(0);
    }

    [Test]
    public async Task NullAttachmentProducesEmptyContent()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        byte[]? capturedContent = null;
        operations.Handler = (_, arguments) => { capturedContent = Read((Stream)arguments[1]!); return null; };
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));

        new AllureAttachmentAspect().AttachReturnValue(
            nameof(DefaultTextAttachment), Method(nameof(DefaultTextAttachment)), [], typeof(string), null
        );

        await Assert.That(capturedContent).IsEmpty();
    }

    [Test]
    public async Task UnsupportedAttachmentValueThrowsWhenEndpointExists()
    {
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint());

        await Assert.That(() => new AllureAttachmentAspect().AttachReturnValue(
            nameof(DefaultTextAttachment), Method(nameof(DefaultTextAttachment)), [], typeof(int), 42
        )).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task MissingEndpointSkipsAttachmentValidation()
    {
        using var scope = FacadeTestEnvironment.Use();

        new AllureAttachmentAspect().AttachReturnValue(
            nameof(DefaultTextAttachment), Method(nameof(DefaultTextAttachment)), [], typeof(int), 42
        );

        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(1);
    }

    static byte[] Read(Stream stream)
    {
        using var copy = new MemoryStream();
        stream.CopyTo(copy);
        return copy.ToArray();
    }

    static MethodInfo Method(string name) =>
        typeof(AttachmentAspectTests).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!;

    [AllureAttachment(
        "attachment {value}",
        ContentType = "application/example",
        Extension = "data",
        Encoding = "UTF-16"
    )]
    static string TextAttachment(int value) => "content";

    [AllureAttachment]
    static string DefaultTextAttachment() => "content";

    [AllureAttachment(Global = true)]
    static byte[] GlobalAttachment() => [1, 2];
}
