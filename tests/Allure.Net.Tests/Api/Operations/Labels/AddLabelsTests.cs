using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Operations.Labels;

public class AddLabelsTests : ApiOperationTestsBase
{
    [Test]
    public async Task AddLabelsRoutedToEndpoint()
    {
        IEnumerable<Label> labels = [new(){ Name = "foo", Value = "bar" }];
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLabels(labels);

        await Assert.That(endpoint.SyncApi.AddLabels((v) => ReferenceEquals(v, labels))).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public void AddLabelsDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        AllureApi.AddLabels([]);
    }

    [Test]
    public async Task AddLabelsAsyncDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLabelsAsync([]);
    }

    [Test]
    public async Task AddLabelsAsyncWithTokenDoesNotThrowWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        await AllureApi.AddLabelsAsync([], CancellationToken.None);
    }
    [Test]
    public async Task AddLabelsSupportVarArgs()
    {
        var label1 = new Label(){ Name = "foo", Value = "bar" };
        var label2 = new Label(){ Name = "baz", Value = "qux" };
        IEnumerable<Label> labels = [label1, label2];
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        AllureApi.AddLabels(label1, label2);

        await Assert.That(endpoint.SyncApi.AddLabels((v) => v.SequenceEqual(labels))).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLabelsAsyncRoutedToEndpoint()
    {
        IEnumerable<Label> labels = [new(){ Name = "foo", Value = "bar" }];
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLabelsAsync(labels);

        await Assert.That(
            endpoint.AsyncApi.AddLabelsAsync(
                (v) => ReferenceEquals(v, labels),
                CancellationToken.None))
            .WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLabelsAsyncSupportVarargs()
    {
        var label1 = new Label(){ Name = "foo", Value = "bar" };
        var label2 = new Label(){ Name = "baz", Value = "qux" };
        IEnumerable<Label> labels = [label1, label2];
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLabelsAsync(label1, label2);

        await Assert.That(
            endpoint.AsyncApi.AddLabelsAsync(
                (v) => v.SequenceEqual(labels),
                CancellationToken.None))
            .WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLabelsAsyncResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelsAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLabelsAsync();

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }

    [Test]
    public async Task AddLabelsAsyncWithTokenRoutedToEndpoint()
    {
        var ts = new CancellationTokenSource();
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        await AllureApi.AddLabelsAsync([], ts.Token);

        await Assert.That(endpoint.AsyncApi.AddLabelsAsync(Any(), ts.Token)).WasCalled(Times.Once);
        endpoint.AsyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task AddLabelsAsyncWithTokenResultTaskForwardedToCaller()
    {
        TaskCompletionSource tcs = new();
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.AddLabelsAsync(Any(), Any()).ReturnsAsync(tcs.Task);

        var actual = AllureApi.AddLabelsAsync([], default);

        await Assert.That(actual).IsSameReferenceAs(tcs.Task);
    }
}
