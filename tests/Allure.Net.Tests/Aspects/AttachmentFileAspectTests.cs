using System.Reflection;
using Allure.Abstractions;
using Allure.Aspects;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Aspects;

public class AttachmentFileAspectTests
{
    [Test]
    public async Task StringPathUsesFileNameAndExtensionByDefault()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));
        var path = Path.GetFullPath("artifact.json");

        new AllureAttachmentFileAspect().AttachReturnValue(
            nameof(DefaultFile), Method(nameof(DefaultFile)), [], typeof(string), path
        );

        var call = operations.SingleCall;
        await Assert.That(call.Method.Name).IsEqualTo("AddFileAttachment");
        await Assert.That(call.Arguments[0]).IsEqualTo("artifact.json");
        await Assert.That(call.Arguments[1]).IsEqualTo(path);
        await Assert.That(call.Arguments[2]).IsNull();
        await Assert.That(call.Arguments[3]).IsEqualTo(".json");
    }

    [Test]
    public async Task ExplicitNameInterpolatesArgumentsAndUsesContentTypeExtensionFallback()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(
            sync: operations.Instance,
            serializer: new TestParameterSerializer("name")
        ));
        var file = new FileInfo(Path.GetFullPath("artifact"));

        new AllureAttachmentFileAspect().AttachReturnValue(
            nameof(NamedFile), Method(nameof(NamedFile)), [42], typeof(FileInfo), file
        );

        var call = operations.SingleCall;
        await Assert.That(call.Arguments[0]).IsEqualTo("file name:42");
        await Assert.That(call.Arguments[1]).IsEqualTo(file.FullName);
        await Assert.That(call.Arguments[2]).IsEqualTo("application/json");
        await Assert.That(call.Arguments[3]).IsEqualTo(".json");
    }

    [Test]
    public async Task GlobalFileUsesGlobalEndpoint()
    {
        var current = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        var global = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(
            new TestApiEndpoint(sync: current.Instance),
            new TestApiEndpoint(sync: global.Instance)
        );

        new AllureAttachmentFileAspect().AttachReturnValue(
            nameof(GlobalFile), Method(nameof(GlobalFile)), [], typeof(string), "artifact.txt"
        );

        await Assert.That(current.Calls).IsEmpty();
        await Assert.That(global.SingleCall.Method.Name).IsEqualTo("AddGlobalFileAttachment");
        await Assert.That(scope.GlobalResolutionCount).IsEqualTo(1);
    }

    [Test]
    public async Task NullFileDoesNotDispatch()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));

        new AllureAttachmentFileAspect().AttachReturnValue(
            nameof(DefaultFile), Method(nameof(DefaultFile)), [], typeof(string), null
        );

        await Assert.That(operations.Calls).IsEmpty();
    }

    [Test]
    public async Task UnsupportedFileValueThrowsWhenEndpointExists()
    {
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint());

        await Assert.That(() => new AllureAttachmentFileAspect().AttachReturnValue(
            nameof(DefaultFile), Method(nameof(DefaultFile)), [], typeof(int), 42
        )).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task MissingEndpointSkipsFileValidation()
    {
        using var scope = FacadeTestEnvironment.Use();

        new AllureAttachmentFileAspect().AttachReturnValue(
            nameof(DefaultFile), Method(nameof(DefaultFile)), [], typeof(int), 42
        );

        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(1);
    }

    static MethodInfo Method(string name) =>
        typeof(AttachmentFileAspectTests).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!;

    [AllureAttachmentFile]
    static string DefaultFile() => "artifact.json";

    [AllureAttachmentFile("file {value}", ContentType = "application/json")]
    static FileInfo NamedFile(int value) => new("artifact");

    [AllureAttachmentFile(Global = true)]
    static string GlobalFile() => "artifact.txt";
}
