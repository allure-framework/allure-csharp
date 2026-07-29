using Allure.Abstractions;
using TUnit.Assertions.Enums;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Attachments;

public class AddScreenDiffTests : AllureApiTestsBase
{
    [Test]
    public async Task AddScreenDiffStreamsRoutedToEndpoint()
    {
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddScreenDiff(expected, actual, diff);

        await Assert.That(endpoint.SyncApi.AddScreenDiff(
            (value) => ReferenceEquals(value, expected),
            (value) => ReferenceEquals(value, actual),
            (value) => ReferenceEquals(value, diff)
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddScreenDiffStreamsDoesNotThrowWithoutEndpoint()
    {
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddScreenDiff(expected, actual, diff);
    }

    [Test]
    public async Task AddScreenDiffStreamsAsyncRoutedToEndpoint()
    {
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddScreenDiffAsync(expected, actual, diff);

        await Assert.That(endpoint.AsyncApi.AddScreenDiffAsync(
            (value) => ReferenceEquals(value, expected),
            (value) => ReferenceEquals(value, actual),
            (value) => ReferenceEquals(value, diff),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddScreenDiffStreamsAsyncResultTaskForwardedToCaller()
    {
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddScreenDiffAsync(Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var result = AllureApi.AddScreenDiffAsync(expected, actual, diff);

        await Assert.That(result).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddScreenDiffStreamsAsyncDoesNotThrowWithoutEndpoint()
    {
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddScreenDiffAsync(expected, actual, diff);
    }

    [Test]
    public async Task AddScreenDiffStreamsAsyncWithTokenRoutedToEndpoint()
    {
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddScreenDiffAsync(expected, actual, diff, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddScreenDiffAsync(
            (value) => ReferenceEquals(value, expected),
            (value) => ReferenceEquals(value, actual),
            (value) => ReferenceEquals(value, diff),
            cts.Token
        )).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddScreenDiffStreamsAsyncWithTokenResultTaskForwardedToCaller()
    {
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddScreenDiffAsync(Any(), Any(), Any(), Any())
            .ReturnsAsync(tcs.Task);

        var result = AllureApi.AddScreenDiffAsync(
            expected,
            actual,
            diff,
            CancellationToken.None
        );

        await Assert.That(result).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddScreenDiffStreamsAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var expected = new MemoryStream([1]);
        using var actual = new MemoryStream([2]);
        using var diff = new MemoryStream([3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddScreenDiffAsync(
            expected,
            actual,
            diff,
            CancellationToken.None
        );
    }

    [Test]
    public async Task AddScreenDiffMemoryRoutedToEndpoint()
    {
        ReadOnlyMemory<byte> expected = new([1]);
        ReadOnlyMemory<byte> actual = new([2]);
        ReadOnlyMemory<byte> diff = new([3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualExpectedBytes = [];
        byte[] actualBytes = [];
        byte[] actualDiffBytes = [];
        endpoint.SyncApi.AddScreenDiff(Any(), Any(), Any()).Callback(
            (expectedStream, actualStream, diffStream) =>
            {
                actualExpectedBytes = ToBytes(expectedStream);
                actualBytes = ToBytes(actualStream);
                actualDiffBytes = ToBytes(diffStream);
            }
        );

        AllureApi.AddScreenDiff(expected, actual, diff);

        await Assert.That(endpoint.SyncApi.AddScreenDiff(
            IsNotNull<Stream>(),
            IsNotNull<Stream>(),
            IsNotNull<Stream>()
        ))
            .WasCalled(Times.Once);
        await Assert.That(actualExpectedBytes)
            .IsEquivalentTo(new byte[]{ 1 }, CollectionOrdering.Matching);
        await Assert.That(actualBytes)
            .IsEquivalentTo(new byte[]{ 2 }, CollectionOrdering.Matching);
        await Assert.That(actualDiffBytes)
            .IsEquivalentTo(new byte[]{ 3 }, CollectionOrdering.Matching);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddScreenDiffMemoryDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> expected = new([1]);
        ReadOnlyMemory<byte> actual = new([2]);
        ReadOnlyMemory<byte> diff = new([3]);
        using var _ = InstallNoEndpoint();

        AllureApi.AddScreenDiff(expected, actual, diff);
    }

    [Test]
    public async Task AddScreenDiffMemoryAsyncRoutedToEndpoint()
    {
        ReadOnlyMemory<byte> expected = new([1]);
        ReadOnlyMemory<byte> actual = new([2]);
        ReadOnlyMemory<byte> diff = new([3]);
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualExpectedBytes = [];
        byte[] actualBytes = [];
        byte[] actualDiffBytes = [];
        endpoint.AsyncApi.AddScreenDiffAsync(Any(), Any(), Any(), Any()).Callback(
            (expectedStream, actualStream, diffStream, _) =>
            {
                actualExpectedBytes = ToBytes(expectedStream);
                actualBytes = ToBytes(actualStream);
                actualDiffBytes = ToBytes(diffStream);
            }
        );

        await AllureApi.AddScreenDiffAsync(expected, actual, diff);

        await Assert.That(endpoint.AsyncApi.AddScreenDiffAsync(
            IsNotNull<Stream>(),
            IsNotNull<Stream>(),
            IsNotNull<Stream>(),
            CancellationToken.None
        )).WasCalled(Times.Once);
        await Assert.That(actualExpectedBytes)
            .IsEquivalentTo(new byte[]{ 1 }, CollectionOrdering.Matching);
        await Assert.That(actualBytes)
            .IsEquivalentTo(new byte[]{ 2 }, CollectionOrdering.Matching);
        await Assert.That(actualDiffBytes)
            .IsEquivalentTo(new byte[]{ 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddScreenDiffMemoryAsyncDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> expected = new([1]);
        ReadOnlyMemory<byte> actual = new([2]);
        ReadOnlyMemory<byte> diff = new([3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddScreenDiffAsync(expected, actual, diff);
    }

    [Test]
    public async Task AddScreenDiffMemoryAsyncWithTokenRoutedToEndpoint()
    {
        ReadOnlyMemory<byte> expected = new([1]);
        ReadOnlyMemory<byte> actual = new([2]);
        ReadOnlyMemory<byte> diff = new([3]);
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        byte[] actualExpectedBytes = [];
        byte[] actualBytes = [];
        byte[] actualDiffBytes = [];
        endpoint.AsyncApi.AddScreenDiffAsync(Any(), Any(), Any(), Any()).Callback(
            (expectedStream, actualStream, diffStream, _) =>
            {
                actualExpectedBytes = ToBytes(expectedStream);
                actualBytes = ToBytes(actualStream);
                actualDiffBytes = ToBytes(diffStream);
            }
        );

        await AllureApi.AddScreenDiffAsync(expected, actual, diff, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddScreenDiffAsync(
            IsNotNull<Stream>(),
            IsNotNull<Stream>(),
            IsNotNull<Stream>(),
            cts.Token
        )).WasCalled(Times.Once);
        await Assert.That(actualExpectedBytes)
            .IsEquivalentTo(new byte[]{ 1 }, CollectionOrdering.Matching);
        await Assert.That(actualBytes)
            .IsEquivalentTo(new byte[]{ 2 }, CollectionOrdering.Matching);
        await Assert.That(actualDiffBytes)
            .IsEquivalentTo(new byte[]{ 3 }, CollectionOrdering.Matching);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddScreenDiffMemoryAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        ReadOnlyMemory<byte> expected = new([1]);
        ReadOnlyMemory<byte> actual = new([2]);
        ReadOnlyMemory<byte> diff = new([3]);
        using var _ = InstallNoEndpoint();

        await AllureApi.AddScreenDiffAsync(
            expected,
            actual,
            diff,
            CancellationToken.None
        );
    }

    static byte[] ToBytes(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}
