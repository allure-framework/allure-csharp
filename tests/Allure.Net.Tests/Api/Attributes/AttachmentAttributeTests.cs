using System.Reflection;
using System.Text;
using Allure.Abstractions;
using Allure.Net.Tests.Infrastructure;
using TUnit.Assertions.Enums;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Attributes;

public class AttachmentAttributeTests : ApiOperationTestsBase
{
    [Test]
    public async Task AttributeHasExpectedUsage()
    {
        var usage = typeof(AllureAttachmentAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;

        await Assert.That(usage.ValidOn).IsEqualTo(AttributeTargets.Method);
        await Assert.That(usage.AllowMultiple).IsFalse();
        await Assert.That(usage.Inherited).IsTrue();
    }

    [Test]
    public async Task DefaultConstructorSetsNameToNull()
    {
        await Assert.That(new AllureAttachmentAttribute().Name).IsNull();
    }

    [Test]
    public async Task NamedConstructorPreservesName()
    {
        await Assert.That(new AllureAttachmentAttribute("Attachment name").Name)
            .IsEqualTo("Attachment name");
    }

    [Test]
    public async Task InitializersPreserveAttributeOptions()
    {
        var attribute = new AllureAttachmentAttribute
        {
            ContentType = "application/example",
            Extension = ".example",
            Encoding = "UTF-16",
            Global = true,
        };

        await Assert.That(attribute.ContentType).IsEqualTo("application/example");
        await Assert.That(attribute.Extension).IsEqualTo(".example");
        await Assert.That(attribute.Encoding).IsEqualTo("UTF-16");
        await Assert.That(attribute.Global).IsTrue();
    }

    [Test]
    public async Task RegularAttachmentForwardsAllComputedValues()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer("argument")
        );
        var captured = CaptureRegular(endpoint);

        var result = CompleteAttachment(17);

        await Assert.That(result).IsEqualTo("Attachment body");
        await Assert.That(Encoding.UTF8.GetString(captured.Content)).IsEqualTo(
            "Attachment body"
        );
        await Assert.That(endpoint.SyncApi.AddAttachment(
            "Attachment argument:17",
            IsNotNull<Stream>(),
            "application/example",
            ".example"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task GlobalAttachmentForwardsAllComputedValues()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Global,
            new TestParameterSerializer("argument")
        );
        var captured = CaptureGlobal(endpoint);

        var result = CompleteGlobalAttachment(18);

        await Assert.That(result).IsEqualTo("Global body");
        await Assert.That(Encoding.UTF8.GetString(captured.Content)).IsEqualTo(
            "Global body"
        );
        await Assert.That(endpoint.SyncApi.AddGlobalAttachment(
            "Global argument:18",
            IsNotNull<Stream>(),
            "text/global",
            ".global"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task UnnamedAttachmentUsesMethodName()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = UnnamedAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachment(
            nameof(UnnamedAttachment),
            IsNotNull<Stream>(),
            "text/plain",
            ".txt"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task EmptyAttachmentNameUsesMethodName()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = EmptyNamedAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachment(
            nameof(EmptyNamedAttachment),
            IsNotNull<Stream>(),
            "text/plain",
            ".txt"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task AttachmentNameInterpolatesMultipleArguments()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer("value")
        );

        _ = InterpolatedAttachment(17, "text");

        await Assert.That(endpoint.SyncApi.AddAttachment(
            "value:17 value:text",
            IsNotNull<Stream>(),
            "text/plain",
            ".txt"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task NameInterpolationUsesOriginalParameterName()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer("value")
        );

        _ = RenamedParameterAttachment(17);

        await Assert.That(endpoint.SyncApi.AddAttachment(
            "value:17",
            IsNotNull<Stream>(),
            "text/plain",
            ".txt"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task IgnoredParameterCanBeUsedForNameInterpolation()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer("value")
        );

        _ = IgnoredParameterAttachment(17);

        await Assert.That(endpoint.SyncApi.AddAttachment(
            "value:17",
            IsNotNull<Stream>(),
            "text/plain",
            ".txt"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task StringDefaultsToTextPlainAndTxtExtension()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = UnnamedAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachment(
            nameof(UnnamedAttachment),
            IsNotNull<Stream>(),
            "text/plain",
            ".txt"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task ByteArrayDefaultsToNullContentTypeAndEmptyExtension()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = ByteArrayAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachment(
            nameof(ByteArrayAttachment),
            IsNotNull<Stream>(),
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task ReadOnlyMemoryDefaultsToNullContentTypeAndEmptyExtension()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = MemoryAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachment(
            nameof(MemoryAttachment),
            IsNotNull<Stream>(),
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task ExplicitContentTypeDerivesExtension()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = JsonAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachment(
            nameof(JsonAttachment),
            IsNotNull<Stream>(),
            "application/json",
            ".json"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task ExtensionWithoutDotGetsDotPrefix()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = ExtensionWithoutDotAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachment(
            nameof(ExtensionWithoutDotAttachment),
            IsNotNull<Stream>(),
            IsNull<string?>(),
            ".data"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task ExtensionStartingWithDotIsPreserved()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = ExtensionWithDotAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachment(
            nameof(ExtensionWithDotAttachment),
            IsNotNull<Stream>(),
            IsNull<string?>(),
            ".data"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task EmptyExplicitExtensionIsPreserved()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = EmptyExtensionAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachment(
            nameof(EmptyExtensionAttachment),
            IsNotNull<Stream>(),
            "application/json",
            ""
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task UnknownContentTypeProducesBinExtension()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = UnknownContentTypeAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachment(
            nameof(UnknownContentTypeAttachment),
            IsNotNull<Stream>(),
            "application/unknown",
            ".bin"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task NullReturnValueProducesEmptyContent()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        var captured = CaptureRegular(endpoint);

        _ = NullAttachment();

        await Assert.That(captured.Content).IsEmpty();
    }

    [Test]
    public async Task ByteArrayProducesIdenticalContent()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        var captured = CaptureRegular(endpoint);

        var result = ByteArrayAttachment();

        await Assert.That(result).IsEquivalentTo(
            new byte[] { 1, 2, 3 },
            CollectionOrdering.Matching
        );
        await Assert.That(captured.Content).IsEquivalentTo(
            result,
            CollectionOrdering.Matching
        );
    }

    [Test]
    public async Task ReadOnlyMemorySliceProducesOnlySlicedContent()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        var captured = CaptureRegular(endpoint);

        var result = MemoryAttachment();

        await Assert.That(result.ToArray()).IsEquivalentTo(
            new byte[] { 2, 3 },
            CollectionOrdering.Matching
        );
        await Assert.That(captured.Content).IsEquivalentTo(
            new byte[] { 2, 3 },
            CollectionOrdering.Matching
        );
    }

    [Test]
    public async Task StringUsesUtf8ByDefault()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        var captured = CaptureRegular(endpoint);

        _ = Utf8Attachment();

        await Assert.That(Encoding.UTF8.GetString(captured.Content)).IsEqualTo(
            "Hello 🌍"
        );
    }

    [Test]
    public async Task StringUsesConfiguredEncoding()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        var captured = CaptureRegular(endpoint);

        _ = Utf16Attachment();

        await Assert.That(Encoding.Unicode.GetString(captured.Content)).IsEqualTo("Hello");
    }

    [Test]
    public async Task CallerStreamIsPassedAndItsPositionRestored()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        Stream? capturedStream = null;
        endpoint.SyncApi.AddAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _) =>
            {
                capturedStream = stream;
                _ = ToBytes(stream);
            }
        );
        using var stream = new MemoryStream([1, 2, 3, 4]);
        stream.Position = 2;

        var result = StreamAttachment(stream);

        await Assert.That(result).IsSameReferenceAs(stream);
        await Assert.That(capturedStream).IsSameReferenceAs(stream);
        await Assert.That(stream.Position).IsEqualTo(2);
        await Assert.That(stream.CanRead).IsTrue();
    }

    [Test]
    public async Task CallerStreamPositionIsRestoredWhenDispatchThrows()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.AddAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _) =>
            {
                _ = stream.ReadByte();
                throw new DispatchException();
            }
        );
        using var stream = new MemoryStream([1, 2, 3]);
        stream.Position = 1;

        await Assert.That(() => StreamAttachment(stream)).Throws<DispatchException>();
        await Assert.That(stream.Position).IsEqualTo(1);
        await Assert.That(stream.CanRead).IsTrue();
    }

    [Test]
    public async Task GeneratedStreamIsDisposedAfterDispatch()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        Stream? capturedStream = null;
        endpoint.SyncApi.AddAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _) => capturedStream = stream
        );

        _ = ByteArrayAttachment();

        await Assert.That(() => capturedStream!.ReadByte()).Throws<ObjectDisposedException>();
    }

    [Test]
    public async Task UnsupportedReturnValueThrowsAfterMethodExecution()
    {
        CallCounter calls = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await Assert.That(() => UnsupportedAttachment(calls)).Throws<InvalidOperationException>();
        await Assert.That(calls.Value).IsEqualTo(1);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UnsupportedReturnValueIsIgnoredWithoutEndpoint()
    {
        CallCounter calls = new();
        using var _ = InstallNoEndpoint();

        var result = UnsupportedAttachment(calls);

        await Assert.That(result).IsEqualTo(17);
        await Assert.That(calls.Value).IsEqualTo(1);
    }

    [Test]
    public async Task InvalidEncodingThrowsAfterMethodExecution()
    {
        CallCounter calls = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await Assert.That(() => InvalidEncodingAttachment(calls)).Throws<ArgumentException>();
        await Assert.That(calls.Value).IsEqualTo(1);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TaskOfStringReturnsResultAndCreatesAttachment()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        var captured = CaptureRegular(endpoint);

        var result = await AsyncStringAttachment();

        await Assert.That(result).IsEqualTo("Async body");
        await Assert.That(Encoding.UTF8.GetString(captured.Content)).IsEqualTo(result);
    }

    [Test]
    public async Task ValueTaskOfReadOnlyMemoryReturnsResultAndCreatesAttachment()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        var captured = CaptureRegular(endpoint);

        var result = await AsyncMemoryAttachment();

        await Assert.That(result.ToArray()).IsEquivalentTo(
            new byte[] { 4, 5 },
            CollectionOrdering.Matching
        );
        await Assert.That(captured.Content).IsEquivalentTo(
            new byte[] { 4, 5 },
            CollectionOrdering.Matching
        );
    }

    [Test]
    public async Task FaultedTaskDoesNotCreateAttachment()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await Assert.That(() => (Task)FaultedAttachment()).Throws<AttachmentMethodException>();
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AttributedMethodExecutesWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        var result = CompleteAttachment(17);

        await Assert.That(result).IsEqualTo("Attachment body");
    }

    private static CapturedContent CaptureRegular(EndpointMocks<
        IAllureOperations_TStepContext_TFixtureContext_Mock<IAllureStepContext, IAllureFixtureContext>,
        IAllureAsyncOperations_TStepContext_TFixtureContext_Mock<IAllureAsyncStepContext, IAllureAsyncFixtureContext>
    > endpoint)
    {
        CapturedContent captured = new();
        endpoint.SyncApi.AddAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _) =>
            {
                captured.Content = ToBytes(stream);
            }
        );
        return captured;
    }

    private static CapturedContent CaptureGlobal(EndpointMocks<
        IAllureOperations_TStepContext_TFixtureContext_Mock<IAllureStepContext, IAllureFixtureContext>,
        IAllureAsyncOperations_TStepContext_TFixtureContext_Mock<IAllureAsyncStepContext, IAllureAsyncFixtureContext>
    > endpoint)
    {
        CapturedContent captured = new();
        endpoint.SyncApi.AddGlobalAttachment(Any(), Any(), Any(), Any()).Callback(
            (_, stream, _, _) =>
            {
                captured.Content = ToBytes(stream);
            }
        );
        return captured;
    }

    private static byte[] ToBytes(Stream stream)
    {
        using var copy = new MemoryStream();
        stream.CopyTo(copy);
        return copy.ToArray();
    }

    private sealed class CapturedContent
    {
        public byte[] Content { get; set; } = [];
    }

    [AllureAttachment(
        "Attachment {value}",
        ContentType = "application/example",
        Extension = "example"
    )]
    private static string CompleteAttachment(int value) => "Attachment body";

    [AllureAttachment(
        "Global {value}",
        ContentType = "text/global",
        Extension = ".global",
        Global = true
    )]
    private static string CompleteGlobalAttachment(int value) => "Global body";

    [AllureAttachment]
    private static string UnnamedAttachment() => "Body";

    [AllureAttachment("")]
    private static string EmptyNamedAttachment() => "Body";

    [AllureAttachment("{first} {second}")]
    private static string InterpolatedAttachment(int first, string second) => "Body";

    [AllureAttachment("{value}")]
    private static string RenamedParameterAttachment(
        [AllureParameter(Name = "renamed")] int value
    ) => "Body";

    [AllureAttachment("{value}")]
    private static string IgnoredParameterAttachment(
        [AllureParameter(Ignore = true)] int value
    ) => "Body";

    [AllureAttachment]
    private static byte[] ByteArrayAttachment() => [1, 2, 3];

    [AllureAttachment]
    private static ReadOnlyMemory<byte> MemoryAttachment() =>
        new byte[] { 1, 2, 3, 4 }.AsMemory(1, 2);

    [AllureAttachment(ContentType = "application/json")]
    private static byte[] JsonAttachment() => [1];

    [AllureAttachment(Extension = "data")]
    private static byte[] ExtensionWithoutDotAttachment() => [1];

    [AllureAttachment(Extension = ".data")]
    private static byte[] ExtensionWithDotAttachment() => [1];

    [AllureAttachment(ContentType = "application/json", Extension = "")]
    private static byte[] EmptyExtensionAttachment() => [1];

    [AllureAttachment(ContentType = "application/unknown")]
    private static byte[] UnknownContentTypeAttachment() => [1];

    [AllureAttachment]
    private static string? NullAttachment() => null;

    [AllureAttachment]
    private static string Utf8Attachment() => "Hello 🌍";

    [AllureAttachment(Encoding = "UTF-16")]
    private static string Utf16Attachment() => "Hello";

    [AllureAttachment]
    private static Stream StreamAttachment(Stream stream) => stream;

    [AllureAttachment]
    private static int UnsupportedAttachment(CallCounter calls)
    {
        calls.Value++;
        return 17;
    }

    [AllureAttachment(Encoding = "not-an-encoding")]
    private static string InvalidEncodingAttachment(CallCounter calls)
    {
        calls.Value++;
        return "Body";
    }

    [AllureAttachment]
    private static async Task<string> AsyncStringAttachment() => "Async body";

    [AllureAttachment]
    private static async ValueTask<ReadOnlyMemory<byte>> AsyncMemoryAttachment() =>
        new byte[] { 4, 5 };

    [AllureAttachment]
    private static async Task<string> FaultedAttachment() =>
        await Task.FromException<string>(new AttachmentMethodException());

    private sealed class DispatchException : Exception;
    private sealed class AttachmentMethodException : Exception;
    private sealed class CallCounter
    {
        public int Value { get; set; }
    }
}
