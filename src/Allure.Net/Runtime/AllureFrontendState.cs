using System;
using Allure.Abstractions;

namespace Allure.Runtime;

/// <summary>
/// Manages initialization and access for an Allure API frontend.
/// </summary>
public sealed class AllureFrontendState(IAllureApiClient defaultClient)
{
    readonly object monitor = new();

    IAllureApiClient client = defaultClient;
    bool provided = false;
    bool frozen = false;

    /// <summary>
    /// Gets the configured client and prevents further preparation.
    /// </summary>
    public IAllureApiClient Client
    {
        get
        {
            lock (this.monitor)
            {
                this.frozen = true;
                return this.client;
            }
        }
    }

    /// <summary>
    /// Gets the sync in-process operations for the current scope.
    /// </summary>
    public IAllureInProcessOperations? InProcessApi =>
        this.Client.ResolveCurrentScope() is { Operations.Sync: var api }
            ? (api as IAllureInProcessOperations
                ?? throw new InvalidOperationException(
                    $"The in-process test API is not supported by '{Client.Name}'."
                ))
            : null;

    /// <summary>
    /// Gets the async in-process operations for the current scope.
    /// </summary>
    public IAllureAsyncInProcessOperations? AsyncInProcessApi =>
        this.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? (api as IAllureAsyncInProcessOperations
                ?? throw new InvalidOperationException(
                    $"The in-process test API is not supported by '{Client.Name}'."
                ))
            : null;

    /// <summary>
    /// Configures the client before this frontend state is first accessed.
    /// </summary>
    public void PrepareClient(IAllureApiClient client)
    {
        if (client is null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        lock (this.monitor)
        {
            if (this.frozen)
            {
                throw new InvalidOperationException(
                    "Allure API client preparation failed: the current client is already in use."
                );
            }

            if (this.provided)
            {
                throw new InvalidOperationException(
                    "Allure API client preparation failed: a client has already been prepared."
                );
            }

            this.client = client;
            provided = true;
        }
    }
}
