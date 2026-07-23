using System.Reflection;
using Allure.Abstractions;
using Allure.Net.Tests.Infrastructure;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Attributes;

public class AttachmentFileAttributeTests : AllureApiTestsBase
{
    [Test]
    public async Task AttributeHasExpectedUsage()
    {
        var usage = typeof(AllureAttachmentFileAttribute)
            .GetCustomAttribute<AttributeUsageAttribute>()!;

        await Assert.That(usage.ValidOn).IsEqualTo(AttributeTargets.Method);
        await Assert.That(usage.AllowMultiple).IsFalse();
        await Assert.That(usage.Inherited).IsTrue();
    }

    [Test]
    public async Task DefaultConstructorSetsNameToNull()
    {
        await Assert.That(new AllureAttachmentFileAttribute().Name).IsNull();
    }

    [Test]
    public async Task NamedConstructorPreservesName()
    {
        await Assert.That(new AllureAttachmentFileAttribute("Attachment name").Name)
            .IsEqualTo("Attachment name");
    }

    [Test]
    public async Task InitializersPreserveAttributeOptions()
    {
        var attribute = new AllureAttachmentFileAttribute
        {
            ContentType = "application/example",
            Global = true,
        };

        await Assert.That(attribute.ContentType).IsEqualTo("application/example");
        await Assert.That(attribute.Global).IsTrue();
    }

    [Test]
    public async Task RegularAttachmentForwardsAllComputedValues()
    {
        using var endpoint = InstallEndpoint(
            InstallationScope.Current,
            new TestParameterSerializer("argument")
        );

        var result = CompleteAttachment(17);

        await Assert.That(result).IsEqualTo(FilePath("attachment.example"));
        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "Attachment argument:17",
            FilePath("attachment.example"),
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

        var result = CompleteGlobalAttachment(18);

        await Assert.That(result.FullName).IsEqualTo(FilePath("global.data"));
        await Assert.That(endpoint.SyncApi.AddGlobalAttachmentFromFile(
            "Global argument:18",
            FilePath("global.data"),
            "text/global",
            ".data"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task UnnamedAttachmentUsesFileName()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = UnnamedAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "unnamed.json",
            FilePath("unnamed.json"),
            IsNull<string?>(),
            ".json"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task EmptyAttachmentNameUsesFileName()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = EmptyNamedAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "empty-name.txt",
            FilePath("empty-name.txt"),
            IsNull<string?>(),
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

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "value:17 value:text",
            FilePath("interpolated.bin"),
            IsNull<string?>(),
            ".bin"
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

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "value:17",
            FilePath("renamed.bin"),
            IsNull<string?>(),
            ".bin"
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

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "value:17",
            FilePath("ignored.bin"),
            IsNull<string?>(),
            ".bin"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task FileExtensionTakesPrecedenceOverContentType()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = ContentTypeWithExtensionAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "content.example",
            FilePath("content.example"),
            "application/json",
            ".example"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task ContentTypeDeterminesMissingFileExtension()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = ContentTypeWithoutExtensionAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "content",
            FilePath("content"),
            "application/json",
            ".json"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task MissingContentTypeAndFileExtensionProducesEmptyExtension()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = NoExtensionAttachment();

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "no-extension",
            FilePath("no-extension"),
            IsNull<string?>(),
            ""
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task NullReturnValueDoesNotCreateAttachment()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        var result = NullAttachment();

        await Assert.That(result).IsNull();
        endpoint.SyncApi.VerifyNoOtherCalls();
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
    public async Task TaskOfStringReturnsResultAndCreatesAttachment()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        var result = await AsyncStringAttachment();

        await Assert.That(result).IsEqualTo(FilePath("async.txt"));
        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "async.txt",
            FilePath("async.txt"),
            IsNull<string?>(),
            ".txt"
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task ValueTaskOfFileInfoReturnsResultAndCreatesAttachment()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        var result = await AsyncFileInfoAttachment();

        await Assert.That(result.FullName).IsEqualTo(FilePath("value-task.data"));
        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "value-task.data",
            FilePath("value-task.data"),
            IsNull<string?>(),
            ".data"
        )).WasCalled(Times.Once);
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

        await Assert.That(result).IsEqualTo(FilePath("attachment.example"));
    }

    [Test]
    public async Task ShouldSerializeArgumentsNoMoreThanOnce()
    {
        ToStringCounter counter = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        MultipleInterpolationsOfSameParameter(counter);

        await Assert.That(endpoint.SyncApi.AddAttachmentFromFile(
            "serialized:1 serialized:1",
            Any(),
            Any(),
            Any()
        )).WasCalled(Times.Once);
        await Assert.That(counter.Value).IsEqualTo(1);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ShouldNotSerializeArgumentsIfNoEndpoint()
    {
        ToStringCounter counter = new();
        using var _ = InstallNoEndpoint();

        MultipleInterpolationsOfSameParameter(counter);

        await Assert.That(counter.Value).IsZero();
    }

    private static string FilePath(string fileName) =>
        Path.Combine(Path.GetTempPath(), fileName);

    [AllureAttachmentFile(
        "Attachment {value}",
        ContentType = "application/example"
    )]
    private static string CompleteAttachment(int value) => FilePath("attachment.example");

    [AllureAttachmentFile(
        "Global {value}",
        ContentType = "text/global",
        Global = true
    )]
    private static FileInfo CompleteGlobalAttachment(int value) =>
        new(FilePath("global.data"));

    [AllureAttachmentFile]
    private static string UnnamedAttachment() => FilePath("unnamed.json");

    [AllureAttachmentFile("")]
    private static string EmptyNamedAttachment() => FilePath("empty-name.txt");

    [AllureAttachmentFile("{first} {second}")]
    private static string InterpolatedAttachment(int first, string second) =>
        FilePath("interpolated.bin");

    [AllureAttachmentFile("{value}")]
    private static string RenamedParameterAttachment(
        [AllureParameter(Name = "renamed")] int value
    ) => FilePath("renamed.bin");

    [AllureAttachmentFile("{value}")]
    private static string IgnoredParameterAttachment(
        [AllureParameter(Ignore = true)] int value
    ) => FilePath("ignored.bin");

    [AllureAttachmentFile(ContentType = "application/json")]
    private static string ContentTypeWithExtensionAttachment() =>
        FilePath("content.example");

    [AllureAttachmentFile(ContentType = "application/json")]
    private static string ContentTypeWithoutExtensionAttachment() => FilePath("content");

    [AllureAttachmentFile]
    private static string NoExtensionAttachment() => FilePath("no-extension");

    [AllureAttachmentFile]
    private static string? NullAttachment() => null;

    [AllureAttachmentFile]
    private static int UnsupportedAttachment(CallCounter calls)
    {
        calls.Value++;
        return 17;
    }

    [AllureAttachmentFile]
    private static async Task<string> AsyncStringAttachment()
    {
        return await Task.FromResult(FilePath("async.txt"));
    }

    [AllureAttachmentFile]
    private static async ValueTask<FileInfo> AsyncFileInfoAttachment()
    {
        return await ValueTask.FromResult(new FileInfo(FilePath("value-task.data")));
    }

    [AllureAttachmentFile]
    private static async Task<string> FaultedAttachment()
    {
        return await Task.FromException<string>(new AttachmentMethodException());
    }

    [AllureAttachmentFile("{counter} {counter}")]
    static string MultipleInterpolationsOfSameParameter(ToStringCounter counter)
    {
        return FilePath("multiinterpolations.data");
    }

    private sealed class AttachmentMethodException : Exception;

    private sealed class CallCounter
    {
        public int Value { get; set; }
    }

    sealed class ToStringCounter
    {
        public int Value { get; set; }
        public override string? ToString()
        {
            this.Value++;
            return this.Value.ToString();
        }
    }
}
