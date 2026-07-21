using Allure.Abstractions;
using Allure.Net.Tests.Infrastructure;
using Allure.Runtime;

namespace Allure.Net.Tests.Runtime;

public class FrontendStateTests
{
    [Test]
    public async Task UsesDefaultClientBeforePreparation()
    {
        var defaultClient = new TestApiClient("default");
        var state = new AllureFrontendState(defaultClient);

        await Assert.That(state.Client).IsSameReferenceAs(defaultClient);
    }

    [Test]
    public async Task UsesPreparedClient()
    {
        var state = new AllureFrontendState(new TestApiClient("default"));
        var preparedClient = new TestApiClient("prepared");

        state.PrepareClient(preparedClient);

        await Assert.That(state.Client).IsSameReferenceAs(preparedClient);
    }

    [Test]
    public async Task RejectsNullClient()
    {
        var state = new AllureFrontendState(new TestApiClient("default"));

        await Assert.That(() => state.PrepareClient(null!))
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task RejectsSecondPreparedClient()
    {
        var state = new AllureFrontendState(new TestApiClient("default"));
        state.PrepareClient(new TestApiClient("first"));

        await Assert.That(() => state.PrepareClient(new TestApiClient("second")))
            .Throws<InvalidOperationException>()
            .WithMessage(
                "Allure API client preparation failed: a client has already been prepared."
            );
    }

    [Test]
    public async Task ReadingClientFreezesState()
    {
        var state = new AllureFrontendState(new TestApiClient("default"));
        _ = state.Client;

        await Assert.That(() => state.PrepareClient(new TestApiClient("prepared")))
            .Throws<InvalidOperationException>()
            .WithMessage(
                "Allure API client preparation failed: the current client is already in use."
            );
    }

    [Test]
    public async Task ReadingInProcessApiFreezesState()
    {
        var sync = InterfaceStub.Create<IAllureInProcessOperations>();
        var endpoint = new TestApiEndpoint(sync: sync);
        var state = new AllureFrontendState(
            new TestApiClient("default", currentScope: () => endpoint)
        );

        await Assert.That(state.InProcessApi).IsSameReferenceAs(sync);
        await Assert.That(() => state.PrepareClient(new TestApiClient("prepared")))
            .Throws<InvalidOperationException>();
    }

    [Test]
    public async Task InProcessApiResolvesCurrentScopeOnlyOnce()
    {
        var endpoint = new TestApiEndpoint(
            sync: InterfaceStub.Create<IAllureInProcessOperations>()
        );
        var client = new TestApiClient("client", currentScope: () => endpoint);
        var state = new AllureFrontendState(client);

        _ = state.InProcessApi;

        await Assert.That(client.CurrentScopeResolutionCount).IsEqualTo(1);
        await Assert.That(client.GlobalScopeResolutionCount).IsEqualTo(0);
    }

    [Test]
    public async Task InProcessApiRejectsUnsupportedEndpoint()
    {
        var endpoint = new TestApiEndpoint();
        var state = new AllureFrontendState(
            new TestApiClient("remote-client", currentScope: () => endpoint)
        );

        await Assert.That(() => _ = state.InProcessApi)
            .Throws<InvalidOperationException>()
            .WithMessage("The in-process test API is not supported by 'remote-client'.");
    }

    [Test]
    public async Task InProcessApiRejectsMissingEndpoint()
    {
        var state = new AllureFrontendState(new TestApiClient("client"));

        await Assert.That(() => _ = state.InProcessApi)
            .Throws<InvalidOperationException>()
            .WithMessage("The in-process test API is not supported by 'client'.");
    }
}
