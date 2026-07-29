using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Links;

public class AddLinkTests : AllureApiTestsBase
{
    [Test]
    public async Task AddLinkByUrlRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLink("https://example.test/reference/42");

        await Assert.That(endpoint.SyncApi.AddLink((link) => link is
        {
            Url: "https://example.test/reference/42",
            Name: null,
            Type: null,
        })).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddLinkByUrlDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddLink("https://example.test/no-endpoint");
    }

    [Test]
    public async Task AddLinkByUrlAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLinkAsync("https://example.test/no-endpoint");
    }

    [Test]
    public async Task AddLinkByUrlAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLinkAsync(
            "https://example.test/no-endpoint",
            cancellationToken: default
        );
    }

    [Test]
    public void AddLinkByUrlAndNameDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddLink("https://example.test/no-endpoint", "Unavailable endpoint");
    }

    [Test]
    public async Task AddLinkByUrlAndNameAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLinkAsync(
            "https://example.test/no-endpoint",
            "Unavailable endpoint"
        );
    }

    [Test]
    public async Task AddLinkByUrlAndNameAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLinkAsync(
            "https://example.test/no-endpoint",
            "Unavailable endpoint",
            cancellationToken: default
        );
    }

    [Test]
    public void AddLinkByUrlNameAndTypeDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddLink(
            "https://example.test/no-endpoint",
            "Unavailable endpoint",
            "reference"
        );
    }

    [Test]
    public async Task AddLinkByUrlNameAndTypeAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLinkAsync(
            "https://example.test/no-endpoint",
            "Unavailable endpoint",
            "reference"
        );
    }

    [Test]
    public async Task AddLinkByUrlNameAndTypeAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLinkAsync(
            "https://example.test/no-endpoint",
            "Unavailable endpoint",
            "reference",
            CancellationToken.None
        );
    }

    [Test]
    public void AddLinkModelDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddLink(new Link
        {
            Url = "https://example.test/no-endpoint",
            Name = "Unavailable endpoint",
            Type = "reference"
        });
    }

    [Test]
    public async Task AddLinkModelAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLinkAsync(new Link
        {
            Url = "https://example.test/no-endpoint",
            Name = "Unavailable endpoint",
            Type = "reference"
        });
    }

    [Test]
    public async Task AddLinkModelAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLinkAsync(
            new Link
            {
                Url = "https://example.test/no-endpoint",
                Name = "Unavailable endpoint",
                Type = "reference"
            },
            CancellationToken.None
        );
    }
    [Test]
    public async Task AddLinkByUrlAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync("https://example.test/reference/42");

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://example.test/reference/42",
                    Name: null,
                    Type: null,
                },
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync("https://example.test/reference/42");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkByUrlAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync("https://example.test/reference/42", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://example.test/reference/42",
                    Name: null,
                    Type: null,
                },
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync(
            "https://example.test/reference/42",
            cancellationToken: default
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkByUrlAndNameRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLink("https://example.test/reference/42", "Reference page");

        await Assert.That(endpoint.SyncApi.AddLink((link) => link is
        {
            Url: "https://example.test/reference/42",
            Name: "Reference page",
            Type: null,
        })).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlAndNameAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page");

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://example.test/reference/42",
                    Name: "Reference page",
                    Type: null,
                },
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlAndNameAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkByUrlAndNameAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://example.test/reference/42",
                    Name: "Reference page",
                    Type: null,
                },
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlAndNameAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync(
            "https://example.test/reference/42",
            "Reference page",
            cancellationToken: default
        );

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkByUrlNameAndTypeRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLink("https://example.test/reference/42", "Reference page", "documentation");

        await Assert.That(endpoint.SyncApi.AddLink((link) => link is
        {
            Url: "https://example.test/reference/42",
            Name: "Reference page",
            Type: "documentation",
        })).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlNameAndTypeAsyncRoutedToEndpoint()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page", "documentation");

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://example.test/reference/42",
                    Name: "Reference page",
                    Type: "documentation",
                },
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlNameAndTypeAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page", "documentation");

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkByUrlNameAndTypeAsyncWithTokenRoutedToEndpoint()
    {
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page", "documentation", cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (link) => link is
                {
                    Url: "https://example.test/reference/42",
                    Name: "Reference page",
                    Type: "documentation",
                },
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkByUrlNameAndTypeAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync("https://example.test/reference/42", "Reference page", "documentation", default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkModelRoutedToEndpoint()
    {
        Link link = new()
        {
            Url = "https://example.test/reference/42",
            Name = "Reference page",
            Type = "documentation"
        };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLink(link);

        await Assert.That(endpoint.SyncApi.AddLink((value) => ReferenceEquals(value, link)))
            .WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkModelAsyncRoutedToEndpoint()
    {
        Link link = new()
        {
            Url = "https://example.test/reference/42",
            Name = "Reference page",
            Type = "documentation"
        };
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync(link);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (value) => ReferenceEquals(value, link),
                CancellationToken.None
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkModelAsyncResultTaskForwardedToCaller()
    {
        Link link = new()
        {
            Url = "https://example.test/reference/42",
            Name = "Reference page",
            Type = "documentation"
        };
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync(link);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLinkModelAsyncWithTokenRoutedToEndpoint()
    {
        Link link = new()
        {
            Url = "https://example.test/reference/42",
            Name = "Reference page",
            Type = "documentation"
        };
        var cts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLinkAsync(link, cts.Token);

        await Assert.That(
            endpoint.AsyncApi.AddLinkAsync(
                (value) => ReferenceEquals(value, link),
                cts.Token
            )
        ).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLinkModelAsyncWithTokenResultTaskForwardedToCaller()
    {
        Link link = new()
        {
            Url = "https://example.test/reference/42",
            Name = "Reference page",
            Type = "documentation"
        };
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLinkAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLinkAsync(link, default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
