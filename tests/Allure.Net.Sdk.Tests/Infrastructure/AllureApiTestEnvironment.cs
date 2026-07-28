using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Net.Sdk.Tests.Infrastructure;

sealed class AllureApiTestEnvironment
{
    readonly AsyncLocal<bool> isInScope = new();

    AllureApiTestEnvironment(IAllureRuntime<AllureConfiguration> runtime)
    {
        this.Runtime = runtime;
    }

    public IAllureRuntime<AllureConfiguration> Runtime { get; }

    public static AllureApiTestEnvironment Create(
        AllureConfiguration? configuration = null
    )
    {
        AllureApiTestEnvironment? environment = null;
        var runtimeEnvironment = RuntimeTestEnvironment.Create(
            configuration,
            builder => builder.RegisterInProcessEndpoint(
                $"sdk-test-{Guid.NewGuid():N}",
                (_, endpoint) =>
                {
                    endpoint.UseCurrentScopePredicate(
                        _ => environment?.isInScope.Value == true
                    );
                    endpoint.UseGlobalScopePredicate(
                        _ => environment?.isInScope.Value == true
                    );
                }
            )
        );
        environment = new(runtimeEnvironment.Runtime);
        return environment;
    }

    public TResult Run<TResult>(Func<AllureApiTestEnvironment, TResult> action)
    {
        var wasInScope = this.isInScope.Value;
        this.isInScope.Value = true;
        try
        {
            return this.Runtime.ContextApi.GetWithState(
                new AllureExecutionState(),
                _ => action(this)
            );
        }
        finally
        {
            this.isInScope.Value = wasInScope;
        }
    }

    public void Run(Action<AllureApiTestEnvironment> action) =>
        this.Run(environment =>
        {
            action(environment);
            return true;
        });

    public async Task<TResult> RunAsync<TResult>(
        Func<AllureApiTestEnvironment, Task<TResult>> action
    )
    {
        var wasInScope = this.isInScope.Value;
        this.isInScope.Value = true;
        try
        {
            return await this.Runtime.ContextApi.GetWithStateAsync(
                new AllureExecutionState(),
                _ => action(this)
            );
        }
        finally
        {
            this.isInScope.Value = wasInScope;
        }
    }

    public Task RunAsync(Func<AllureApiTestEnvironment, Task> action) =>
        this.RunAsync(async environment =>
        {
            await action(environment);
            return true;
        });
}
