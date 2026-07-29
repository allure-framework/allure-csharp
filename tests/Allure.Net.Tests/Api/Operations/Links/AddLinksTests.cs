using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Links;

public class AddLinksTests : AllureApiTestsBase
{
    [Test]
    public async Task AddLinksRoutedToEndpoint()
    {
        IEnumerable<Link> links =
        [
            new() { Url = "https://example.test/first", Name = "First link", Type = "reference" }
        ];
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLinks(links);

        await Assert.That(endpoint.SyncApi.AddLinks((value) => ReferenceEquals(value, links)))
            .WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddLinksDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddLinks([]);
    }

    [Test]
    public async Task AddLinksAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLinksAsync([]);
    }

    [Test]
    public async Task AddLinksAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLinksAsync([], CancellationToken.None);
    }
    [Test]
    public async Task AddLinksSupportsVarArgs()
    {
        Link first = new()
        {
            Url = "https://example.test/first",
            Name = "First link",
            Type = "reference"
        };
        Link second = new()
        {
            Url = "https://example.test/second",
            Name = "Second link",
            Type = "documentation"
        };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLinks(first, second);

        await Assert.That(
            endpoint.SyncApi.AddLinks(
                (links) => links.SequenceEqual(new[] { first, second })
            )
        ).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinksAsyncRoutedToEndpoint()
    {
        IEnumerable<Link> links =
        [
            new() { Url = "https://example.test/first", Name = "First link", Type = "reference" }
        ];
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinksAsync(links);

        await Assert.That(
            endpoint.AsyncApi.AddLinksAsync(
                (value) => ReferenceEquals(value, links),
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinksAsyncSupportsVarArgs()
    {
        Link first = new()
        {
            Url = "https://example.test/first",
            Name = "First link",
            Type = "reference"
        };
        Link second = new()
        {
            Url = "https://example.test/second",
            Name = "Second link",
            Type = "documentation"
        };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinksAsync(first, second);

        await Assert.That(
            endpoint.AsyncApi.AddLinksAsync(
                (links) => links.SequenceEqual(new[] { first, second }),
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinksAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinksAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinksAsync();

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinksAsyncWithTokenRoutedToEndpoint()
    {
        IEnumerable<Link> links =
        [
            new() { Url = "https://example.test/first", Name = "First link", Type = "reference" }
        ];
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinksAsync(links, cts.Token);

        await Assert.That(endpoint.AsyncApi.AddLinksAsync((value) => ReferenceEquals(value, links), cts.Token))
            .WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinksAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinksAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinksAsync([], default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
