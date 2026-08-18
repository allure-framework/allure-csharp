using System.Collections.Immutable;
using Allure.Sdk.Configuration;
using Allure.Sdk.Results;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities.TestFramework;

namespace Allure.TestingPlatform.Tests.OperationTests;

static class OperationTestApplication
{
    static readonly string[] DefaultArgs =
    [
        "--no-progress",
        "--no-ansi",
        "--output",
        "Normal",
        "--show-stdout",
        "None",
        "--show-stderr",
        "None",
    ];

    public static async Task<InMemoryResultsDestination> RunAsync(
        Func<Task> operation,
        OperationTarget target = OperationTarget.Test,
        IEnumerable<string> failExceptions = null,
        ImmutableDictionary<string, AllureLinkTemplate> linkTemplates = null
    )
    {
        var destination = new InMemoryResultsDestination();
        var executionContext = new OperationExecutionContext();
        var builder = await TestApplication.CreateBuilderAsync(DefaultArgs);

        builder.AddEmbeddedAllure(
            $"operation-test-{Guid.NewGuid():N}",
            (context, _) =>
            {
                context.UseConfiguration(new AllureTestingPlatformConfiguration
                {
                    IsProcessWatchdogEnabled = false,
                    FailExceptions = [.. failExceptions ?? []],
                    LinkTemplates = linkTemplates ?? [],
                });
                context.UseCorrelationContext(_ => executionContext);
                context.UseCorrelationStrategy(_ => executionContext);
                context.UseExecutionStateContext(_ => executionContext);
                context.UseDestination(_ => destination);
            },
            (context, _, _) =>
            {
                context.UseCurrentScopePredicate(_ => executionContext.IsActive);
                context.UseGlobalScopePredicate(_ => executionContext.IsActive);
            }
        );
        builder.RegisterTestFramework(
            _ => new TestFrameworkCapabilities(),
            (_, _) => new OperationTestFrameworkStub(
                executionContext,
                target,
                operation
            )
        );

        using var app = await builder.BuildAsync();
        var exitCode = await app.RunAsync();
        if (exitCode != 0)
        {
            throw new InvalidOperationException(
                $"The operation test application exited with code {exitCode}."
            );
        }

        return destination;
    }
}
